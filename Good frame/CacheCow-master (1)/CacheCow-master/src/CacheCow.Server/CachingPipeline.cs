﻿#if NET452
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CacheCow.Common;
using CacheCow.Server.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace CacheCow.Server
{
    public class CachingPipeline : ICachingPipeline
    {
        private ICacheabilityValidator _validator;
        private readonly ICacheDirectiveProvider _cacheDirectiveProvider;
        private Stream _stream = null;
        private CacheValidationStatus _cacheValidationStatus;
        bool _isRequestCacheable = false;
        bool? _cacheValidated = null;
        private CacheCowHeader _cacheCowHeader;
        private readonly bool _doNotEmitHeader = false;

        public CachingPipeline(ICacheabilityValidator validator,
            ICacheDirectiveProvider cacheDirectiveProvider,
            HttpCachingOptions options)
        {
            _validator = validator;
            _cacheDirectiveProvider = cacheDirectiveProvider;
            _doNotEmitHeader = options.DoNotEmitCacheCowHeader;
        }


        public async Task<bool> Before(HttpContext context)
        {
            _cacheCowHeader = new CacheCowHeader();
            _cacheValidated = null;
            _isRequestCacheable = _validator.IsCacheable(context.Request);
            _cacheValidationStatus = context.Request.GetCacheValidationStatus();
            if (_cacheValidationStatus != CacheValidationStatus.None)
            {
                var timedETag = await _cacheDirectiveProvider.QueryAsync(context);
                _cacheCowHeader.QueryMadeAndSuccessful = timedETag != null;
                _cacheValidated = ApplyCacheValidation(timedETag, _cacheValidationStatus, context);
                _cacheCowHeader.ValidationApplied = true;
                if (_cacheValidated ?? false)
                {
                    _cacheCowHeader.ShortCircuited = true;
                    _cacheCowHeader.ValidationMatched = HttpMethods.IsGet(context.Request.Method); // NOTE: In GET match result in short-circuit and in PUT the opposite
                    if (! _doNotEmitHeader)
                        context.Response.Headers.Add(CacheCowHeader.Name, _cacheCowHeader.ToString());
                    return false;
                }
            }

            _stream = context.Response.Body;
            context.Response.Body = new MemoryStream();

            return true;

        }

        protected bool? ApplyCacheValidation(TimedEntityTagHeaderValue timedEtag,
            CacheValidationStatus cacheValidationStatus,
            HttpContext context)
        {
            if (timedEtag == null)
                return null;

            var headers = context.Request.GetTypedHeadersWithCaching();
            switch (cacheValidationStatus)
            {
                case CacheValidationStatus.GetIfModifiedSince:
                    if (timedEtag.LastModified == null)
                        return false;
                    else
                    {
                        if (timedEtag.LastModified > headers.IfModifiedSince.Value)
                            return false;
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status304NotModified;
                            return true;
                        }
                    }

                case CacheValidationStatus.GetIfNoneMatch:
                    if (timedEtag.ETag == null)
                        return false;
                    else
                    {
                        if (headers.IfNoneMatch.Any(x => x.Tag == timedEtag.ETag.Tag))
                        {
                            context.Response.StatusCode = StatusCodes.Status304NotModified;
                            return true;
                        }
                        else
                            return false;
                    }
                case CacheValidationStatus.PutPatchDeleteIfMatch:
                    if (timedEtag.ETag == null)
                        return false;
                    else
                    {
                        if (headers.IfMatch.Any(x => x.Tag == timedEtag.ETag.Tag))
                            return false;
                        else
                        {
                            context.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                            return true;
                        }
                    }
                case CacheValidationStatus.PutPatchDeleteIfUnModifiedSince:
                    if (timedEtag.LastModified == null)
                        return false;
                    else
                    {
                        if (timedEtag.LastModified > headers.IfUnmodifiedSince.Value)
                        {
                            context.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
                            return true;
                        }
                        else
                            return false;
                    }

                default:
                    return null;
            }
        }

        public bool ApplyNoCacheNoStoreForNonCacheableResponse { get; set; }
        public TimeSpan? ConfiguredExpiry { get; set; }


        public async Task After(HttpContext context, object viewModel)
        {
            MemoryStream ms = context.Response.Body as MemoryStream;
            bool mustReflush = ms != null && ms.Length > 0;
            context.Response.Body = _stream;

            try
            {
                if (HttpMethods.IsGet(context.Request.Method))
                {
                    context.Response.Headers.Add(HeaderNames.Vary, string.Join(";", _cacheDirectiveProvider.GetVaryHeaders(context)));
                    var cacheControl = _cacheDirectiveProvider.GetCacheControl(context, this.ConfiguredExpiry);
                    var isResponseCacheable = _validator.IsCacheable(context.Response);
                    if (!cacheControl.NoStore && isResponseCacheable) // _______ is cacheable
                    {
                        TimedEntityTagHeaderValue tet = null;
                        if (viewModel != null)
                        {
                            tet = _cacheDirectiveProvider.Extract(viewModel);
                        }

                        if (_cacheValidated == null  // could not validate
                            && tet != null
                            && _cacheValidationStatus != CacheValidationStatus.None) // can only do GET validation, PUT is already impacted backend stores
                        {
                            _cacheValidated = ApplyCacheValidation(tet, _cacheValidationStatus, context);
                            _cacheCowHeader.ValidationApplied = true;

                            if (_cacheValidated ?? false)
                            {
                                _cacheCowHeader.ValidationMatched = true;
                                mustReflush = false; // issue 241 fix. The body should be empty.
                                ms.Dispose();
                                if (! _doNotEmitHeader)
                                    context.Response.Headers.Add(CacheCowHeader.Name, _cacheCowHeader.ToString());
                                return;
                            }
                        }

                        if (tet != null)
                            context.Response.ApplyTimedETag(tet);
                    }

                    if (!_isRequestCacheable || !isResponseCacheable)
                        context.Response.MakeNonCacheable();
                    else
                        context.Response.Headers[HttpHeaderNames.CacheControl] = cacheControl.ToString();
                    if (! _doNotEmitHeader)
                        context.Response.Headers.Add(CacheCowHeader.Name, _cacheCowHeader.ToString());
                }

            }
            finally
            {
                if (mustReflush)
                {
                    ms.Position = 0;
                    await ms.CopyToAsync(context.Response.Body);
                }
            }

        }
    }

    public class CachingPipeline<TViewModel> : CachingPipeline, ICachingPipeline<TViewModel>
    {
        public CachingPipeline(ICacheabilityValidator validator,
            ICacheDirectiveProvider<TViewModel> cacheDirectiveProvider,
            HttpCachingOptions options) : base(validator, cacheDirectiveProvider, options)
        {
        }
    }
}
#endif
