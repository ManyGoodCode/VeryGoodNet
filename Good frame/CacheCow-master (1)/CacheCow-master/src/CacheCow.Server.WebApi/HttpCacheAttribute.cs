﻿using CacheCow.Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;
using System.Linq;
using CacheCow.Server.Headers;

namespace CacheCow.Server.WebApi
{
    public class HttpCacheAttribute : ActionFilterAttribute
    {
        private const string CacheValidatedKey = "###__cache_validated__###";
        private const string CacheCowHeaderKey = "###__cachecow_header__###";
        private bool _doNotEmitHeader = false;
        private IConfigurationValueProvider _configValueProvider = new ConfigurationValueProvider();

        public HttpCacheAttribute()
        {
            ApplyNoCacheNoStoreForNonCacheableResponse = true;
            CachingRuntime.OnHttpCacheCreated(new HttpCacheCreatedEventArgs(this));
            var val = _configValueProvider.GetValue(ConfigurationKeys.DoNotEmitCacheCowHeader);
            bool.TryParse(val, out _doNotEmitHeader);
        }

        protected bool? ApplyCacheValidation(TimedEntityTagHeaderValue timedEtag,
            CacheValidationStatus cacheValidationStatus,
            ContextUnifier context)
        {
            if (timedEtag == null)
                return null;

            switch (cacheValidationStatus)
            {
                case CacheValidationStatus.GetIfModifiedSince:
                    if (timedEtag.LastModified == null)
                        return false;
                    else
                    {
                        if (timedEtag.LastModified > context.Request.Headers.IfModifiedSince.Value)
                            return false;
                        else
                        {
                            context.Response = new HttpResponseMessage(HttpStatusCode.NotModified);
                            return true;
                        }
                    }

                case CacheValidationStatus.GetIfNoneMatch:
                    if (timedEtag.ETag == null)
                        return false;
                    else
                    {
                        if (context.Request.Headers.IfNoneMatch.Any(x => x.Tag == timedEtag.ETag.Tag))
                        {
                            context.Response = new HttpResponseMessage(HttpStatusCode.NotModified);
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
                        if (context.Request.Headers.IfMatch.Any(x => x.Tag == timedEtag.ETag.Tag))
                            return false;
                        else
                        {
                            context.Response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
                            return true;
                        }
                    }
                case CacheValidationStatus.PutPatchDeleteIfUnModifiedSince:
                    if (timedEtag.LastModified == null)
                        return false;
                    else
                    {
                        if (timedEtag.LastModified > context.Request.Headers.IfUnmodifiedSince.Value)
                        {
                            context.Response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
                            return true;
                        }
                        else
                            return false;
                    }

                default:
                    return null;
            }
        }


        public async override Task OnActionExecutingAsync(HttpActionContext context, CancellationToken cancellationToken)
        {
            await base.OnActionExecutingAsync(context, cancellationToken);

            var cacheabilityValidator = (ICacheabilityValidator) context.ControllerContext.Configuration.DependencyResolver.GetService(typeof(ICacheabilityValidator))
                ?? new DefaultCacheabilityValidator();
            var cacheDirectiveProvider = context.ControllerContext.Configuration.DependencyResolver.GetCacheDirectiveProvider(ViewModelType);

            var cacheCowHeader = new CacheCowHeader();
            context.Request.Properties[CacheCowHeaderKey] = cacheCowHeader;
            bool? cacheValidated = null;
            bool isRequestCacheable = cacheabilityValidator.IsCacheable(context.Request);
            var cacheValidationStatus = context.Request.GetCacheValidationStatus();
            if (cacheValidationStatus != CacheValidationStatus.None)
            {
                var timedETag = await cacheDirectiveProvider.QueryAsync(context);
                cacheCowHeader.QueryMadeAndSuccessful = timedETag != null;
                cacheValidated = ApplyCacheValidation(timedETag, cacheValidationStatus, new ContextUnifier(context));
                context.Request.Properties.Add(CacheValidatedKey, cacheValidated);
                cacheCowHeader.ValidationApplied = true;
                if (cacheValidated ?? false)
                {
                    cacheCowHeader.ShortCircuited = true;
                    cacheCowHeader.ValidationMatched = HttpMethod.Get == context.Request.Method; // NOTE: In GET match result in short-circuit and in PUT the opposite
                    if (!_doNotEmitHeader)
                        context.Response.Headers.Add(CacheCowHeader.Name, cacheCowHeader.ToString());
                    // the response would have been set and no need to run the rest of the pipeline
                    return;
                }
            }
        }

        public async override Task OnActionExecutedAsync(HttpActionExecutedContext context, CancellationToken cancellationToken)
        {
            await base.OnActionExecutedAsync(context, cancellationToken);
            var cacheabilityValidator = (ICacheabilityValidator) context.ActionContext.ControllerContext.Configuration.DependencyResolver.GetService(typeof(ICacheabilityValidator))
                ?? new DefaultCacheabilityValidator();
            var cacheDirectiveProvider = context.ActionContext.ControllerContext.Configuration.DependencyResolver.GetCacheDirectiveProvider(ViewModelType);

            bool? cacheValidated = context.Request.Properties.ContainsKey(CacheValidatedKey) ?
                (bool?) context.Request.Properties[CacheValidatedKey] : null;
            var cacheValidationStatus = context.Request.GetCacheValidationStatus();
            var cacheCowHeader = (CacheCowHeader) context.Request.Properties[CacheCowHeaderKey];
            bool isRequestCacheable = cacheabilityValidator.IsCacheable(context.Request);

            if (HttpMethod.Get == context.Request.Method && context.Exception == null)
            {
                context.Response.Headers.Add(HttpHeaderNames.Vary, string.Join(";", cacheDirectiveProvider.GetVaryHeaders(context)));
                var cacheControl = cacheDirectiveProvider.GetCacheControl(context, TimeSpan.FromSeconds(this.DefaultExpirySeconds));
                var isResponseCacheable = cacheabilityValidator.IsCacheable(context.Response);
                if (!cacheControl.NoStore && isResponseCacheable) // _______ is cacheable
                {
                    var or = context.Response.Content as ObjectContent;
                    TimedEntityTagHeaderValue tet = null;
                    if (or != null && or.Value != null)
                    {
                        tet = cacheDirectiveProvider.Extract(or.Value);
                    }

                    if (cacheValidated == null  // could not validate
                        && tet != null
                        && cacheValidationStatus != CacheValidationStatus.None) // can only do GET validation, PUT is already impacted backend stores
                    {
                        cacheValidated = ApplyCacheValidation(tet, cacheValidationStatus, new ContextUnifier(context));
                        cacheCowHeader.ValidationApplied = true;
                        // the response would have been set and no need to run the rest of the pipeline

                        if (cacheValidated ?? false)
                        {
                            cacheCowHeader.ValidationMatched = true;
                            if (!_doNotEmitHeader)
                                context.Response.Headers.Add(CacheCowHeader.Name, cacheCowHeader.ToString());
                            return;
                        }
                    }

                    if (tet != null)
                        context.Response.ApplyTimedETag(tet);
                }

                if ((!isRequestCacheable || !isResponseCacheable) && ApplyNoCacheNoStoreForNonCacheableResponse)
                    context.Response.MakeNonCacheable();
                else
                    context.Response.Headers.Add(HttpHeaderNames.CacheControl, cacheControl.ToString());

                if (! _doNotEmitHeader)
                    context.Response.Headers.Add(CacheCowHeader.Name, cacheCowHeader.ToString());
            }


        }

        public bool ApplyNoCacheNoStoreForNonCacheableResponse { get; set; }
        public int DefaultExpirySeconds { get; set; }
        public Type ViewModelType { get; set; }
    }
}
