using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Application.Features.Sites.Caching
{

    public static class SiteCacheKey
    {
        public const string GetAllCacheKey = "all-Sites";
        public static string GetPagtionCacheKey(string parameters)
        {
            return $"SitesWithPaginationQuery,{parameters}";
        }
        static SiteCacheKey()
        {
            tokensource = new CancellationTokenSource(new TimeSpan(3, 0, 0));
        }

        private static CancellationTokenSource tokensource;

        public static CancellationTokenSource SharedExpiryTokenSource()
        {
            if (tokensource.IsCancellationRequested)
            {
                tokensource = new CancellationTokenSource(new TimeSpan(3, 0, 0));
            }

            return tokensource;
        }

        public static MemoryCacheEntryOptions MemoryCacheEntryOptions
            => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
    }
}

