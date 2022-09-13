using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Caching
{
    public static class VisitorCacheKey
    {
        public const string GetAllCacheKey = "all-Visitors";
        public const string GetKanbanCacheKey = "KanbanVisitors";
        public const string GetSummaryCacheKey = "SummaryVisitors";
        public const string GetCountedMonthlyCacheKey = "CountedMonthlyVisitors";
        public const string GetCountedPurposeCacheKey = "CountedPurposeVisitors";
        public static string GetPagtionCacheKey(string parameters)
        {
            return $"VisitorsWithPaginationQuery,{parameters}";
        }

        public static string GetByIdCacheKey(int id)
        {
            return $"GetByIdVisitorQuery,{id}";
        }

        public static string GetRelatedCacheKey(int? id)
        {
            return $"GetRelatedVisitorQuery,{id}";
        }

        public static string Search(string keyword)
        {
            return $"SearchVisitorQuery:{keyword}";
        }

        public static string SearchFuzzy(string keyword)
        {
            return $"SearchVisitorFuzzyQuery:{keyword}";
        }

        public static string SearchPendingApproval(string keyword)
        {
            return $"SearchPendingApprovalVisitorsQuery:{keyword}";
        }

        public static string SearchPendingChecking(string keyword)
        {
            return $"SearchPendingCheckingVisitorsQuery:{keyword}";
        }

        public static string SearchPendingCheckin(string keyword)
        {
            return $"SearchPendingCheckinVisitorsQuery:{keyword}";
        }

        public static string SearchPendingConfirm(string keyword)
        {
            return $"SearchPendingConfirmVisitorsQuery:{keyword}";
        }

        static VisitorCacheKey()
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

