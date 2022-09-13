using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Application.Features.VisitorHistories.Caching
{
    public static class VisitorHistoryCacheKey
    {
        public const string GetAllCacheKey = "all-VisitorHistories";

        public static string GetPagtionCacheKey(string parameters)
        {
            return $"VisitorHistoriesWithPaginationQuery,{parameters}";
        }

        static VisitorHistoryCacheKey()
        {
            tokensource = new CancellationTokenSource(new TimeSpan(3, 0, 0));
        }

        public static string GetByVisitorIdCacheKey(int? id)
        {
            return $"GetByVisitorIdCacheKey-{id}";
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

