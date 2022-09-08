using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Caching
{
    /// <summary>
    /// 1. 定义缓存键值。如：所有缓存键值，页面访问键值(传入页面名称参数获取)，访问者Id键值(传入访问者Id)
    /// 2. 设置缓存最长事件3小时以及刷新缓存时间
    /// </summary>
    public static class ApprovalHistoryCacheKey
    {
        public const string GetAllCacheKey = "all-ApprovalHistories";
        public static string GetPagtionCacheKey(string parameters) => $"ApprovalHistoriesWithPaginationQuery,{parameters}";
        public static string GetByVisitorIdCacheKey(int id) => $"GetByVisitorIdCacheKey,{id}";
        private static System.Threading.CancellationTokenSource tokensource;

        static ApprovalHistoryCacheKey()
        {
            tokensource = new CancellationTokenSource(new System.TimeSpan(hours: 3, minutes: 0, seconds: 0));
        }

        public static CancellationTokenSource SharedExpiryTokenSource()
        {
            if (tokensource.IsCancellationRequested)
            {
                tokensource = new CancellationTokenSource(new System.TimeSpan(3, 0, 0));
            }

            return tokensource;
        }

        public static MemoryCacheEntryOptions MemoryCacheEntryOptions => new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(SharedExpiryTokenSource().Token));
    }
}

