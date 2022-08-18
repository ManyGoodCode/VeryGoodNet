#if NET452
using System.Web.Http.Filters;
#else
using Microsoft.AspNetCore.Http;
#endif
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace CacheCow.Server
{
    public interface ICacheDirectiveProvider : ITimedETagExtractor, ITimedETagQueryProvider
    {
#if NET452
        CacheControlHeaderValue GetCacheControl(HttpActionExecutedContext context, TimeSpan? configuredExpiry);
        IEnumerable<string> GetVaryHeaders(HttpActionExecutedContext context);
#else
        CacheControlHeaderValue GetCacheControl(HttpContext context, TimeSpan? configuredExpiry);
        IEnumerable<string> GetVaryHeaders(HttpContext context);
#endif

    }

    public interface ICacheDirectiveProvider<TViewModel> : ICacheDirectiveProvider
    {
    }
}
