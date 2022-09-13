using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Features.Visitors.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Queries.Reports
{

    public class GetDashboardDataQuery : IRequest<Tuple<int, int, int>>, ICacheable
    {
        public string CacheKey => VisitorCacheKey.GetSummaryCacheKey;
        public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;
    }

    public class GetVisitorCountedMonthlyDataQuery : IRequest<List<VisitorCountedMonth>?>, ICacheable
    {
        public string CacheKey => VisitorCacheKey.GetCountedMonthlyCacheKey;
        public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;

    }
    public class GetVisitorCountedPurposeDataQuery : IRequest<Dictionary<string, int>?>, ICacheable
    {
        public string CacheKey => VisitorCacheKey.GetCountedPurposeCacheKey;
        public MemoryCacheEntryOptions? Options => VisitorCacheKey.MemoryCacheEntryOptions;

    }
    public class GetDashboardDataQueryHandler :
         IRequestHandler<GetVisitorCountedPurposeDataQuery, Dictionary<string, int>?>,
         IRequestHandler<GetVisitorCountedMonthlyDataQuery, List<VisitorCountedMonth>?>,
         IRequestHandler<GetDashboardDataQuery, Tuple<int, int, int>>
    {

        private readonly IApplicationDbContext context;
        public GetDashboardDataQueryHandler(
            IApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Tuple<int, int, int>> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            int total = await context.Visitors.CountAsync(cancellationToken);
            int totalcheckin = await context.Visitors.CountAsync(x => x.CheckinDate != null && x.CheckoutDate == null);
            int totalcheckout = await context.Visitors.CountAsync(x => x.CheckinDate != null && x.CheckoutDate != null);
            return new Tuple<int, int, int>(total, totalcheckin, totalcheckout);
        }

        public async Task<List<VisitorCountedMonth>?> Handle(GetVisitorCountedMonthlyDataQuery request, CancellationToken cancellationToken)
        {
            List<VisitorCountedMonth> result = await context.Visitors.GroupBy(x => new
            {
                Month = x.Created.Value.Month,
                Year = x.Created.Value.Year
            })
                .Select(x => new VisitorCountedMonth { Month = x.Key.Month, Year = x.Key.Year, Count = x.Count() })
                .ToListAsync(cancellationToken: cancellationToken);
            return result;
        }

        public async Task<Dictionary<string, int>?> Handle(GetVisitorCountedPurposeDataQuery request, CancellationToken cancellationToken)
        {
            var result = await context.Visitors.Where(x => x.Purpose != null).GroupBy(x => x.Purpose)
                .Select(x => new { Purpose = x.Key, Count = x.Count() })
                .ToDictionaryAsync(x => x.Purpose, x => x.Count, cancellationToken: cancellationToken);
            return result;
        }
    }


    public class VisitorCountedMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
    }
}

