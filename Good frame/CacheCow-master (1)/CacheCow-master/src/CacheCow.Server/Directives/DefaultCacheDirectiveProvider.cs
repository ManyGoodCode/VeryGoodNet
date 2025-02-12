﻿using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CacheCow.Common;
#if NET452
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace CacheCow.Server
{
    public class DefaultCacheDirectiveProvider : ICacheDirectiveProvider
    {
        private readonly ITimedETagExtractor timedETagExtractor;
        private readonly ITimedETagQueryProvider queryProvider;

        public DefaultCacheDirectiveProvider(
            ITimedETagExtractor timedETagExtractor,
            ITimedETagQueryProvider queryProvider)
        {
            this.timedETagExtractor = timedETagExtractor;
            this.queryProvider = queryProvider;
        }

        public TimedEntityTagHeaderValue Extract(object viewModel)
        {
            return timedETagExtractor.Extract(viewModel);
        }

#if NET452
        public Task<TimedEntityTagHeaderValue> QueryAsync(HttpActionContext context)
#else
        public Task<TimedEntityTagHeaderValue> QueryAsync(HttpContext context)
#endif
        {
            return queryProvider.QueryAsync(context);
        }


        public virtual void Dispose()
        {
            queryProvider.Dispose();
        }

#if NET452
        public IEnumerable<string> GetVaryHeaders(HttpActionExecutedContext context)
#else
        public IEnumerable<string> GetVaryHeaders(HttpContext context)
#endif
        {
            return new[] { HttpHeaderNames.Accept };
        }

#if NET452
        public CacheControlHeaderValue GetCacheControl(HttpActionExecutedContext context, TimeSpan? configuredExpiry)
#else
        public CacheControlHeaderValue GetCacheControl(HttpContext context, TimeSpan? configuredExpiry)
#endif
        {
            switch (configuredExpiry)
            {
                case TimeSpan t when t == TimeSpan.Zero:
                    return new CacheControlHeaderValue() { MaxAge = TimeSpan.Zero, Private = true, MustRevalidate = true };
                case TimeSpan t:
                    return new CacheControlHeaderValue() { MaxAge = t, Public = true, MustRevalidate = true };
                default:
                    return new CacheControlHeaderValue() { NoCache = true, NoStore = true };
            }
        }
    }



    public class DefaultCacheDirectiveProvider<TViewModel> : DefaultCacheDirectiveProvider, ICacheDirectiveProvider<TViewModel>
    {
        public DefaultCacheDirectiveProvider(
            ITimedETagExtractor<TViewModel> timedETagExtractor,
            ITimedETagQueryProvider<TViewModel> queryProvider) : base(timedETagExtractor, queryProvider)
        {
        }
    }

}
