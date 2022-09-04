using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Caching
{

    public static class ApprovalHistoryCacheKey
    {
        public const string GetAllCacheKey = "all-ApprovalHistories";
        public static string GetPagtionCacheKey(string parameters) => $"ApprovalHistoriesWithPaginationQuery,{parameters}";

        public static string GetByVisitorIdCacheKey(int id) => $"GetByVisitorIdCacheKey,{id}";

        static ApprovalHistoryCacheKey()
        {
            _tokensource = new CancellationTokenSource(new TimeSpan(3, 0, 0));
        }
        private static CancellationTokenSource _tokensource;
        public static CancellationTokenSource SharedExpiryTokenSource()
        {
            if (_tokensource.IsCancellationRequested)
            {
                _tokensource = new CancellationTokenSource(new TimeSpan(3, 0, 0));
            }
            return _tokensource;
        }
        public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
    }
}

