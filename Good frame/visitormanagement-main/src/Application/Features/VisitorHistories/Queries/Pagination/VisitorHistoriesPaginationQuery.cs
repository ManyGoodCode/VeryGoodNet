using CleanArchitecture.Blazor.Application.Features.VisitorHistories.DTOs;
using CleanArchitecture.Blazor.Application.Features.VisitorHistories.Caching;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;

namespace CleanArchitecture.Blazor.Application.Features.VisitorHistories.Queries.Pagination
{

    public class VisitorHistoriesWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<VisitorHistoryDto>>, ICacheable
    {
        public string CacheKey => VisitorHistoryCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => VisitorHistoryCacheKey.MemoryCacheEntryOptions;
    }

    public class VisitorHistoriesWithPaginationQueryHandler :
         IRequestHandler<VisitorHistoriesWithPaginationQuery, PaginatedData<VisitorHistoryDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<VisitorHistoriesWithPaginationQueryHandler> localizer;

        public VisitorHistoriesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<VisitorHistoriesWithPaginationQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<PaginatedData<VisitorHistoryDto>> Handle(VisitorHistoriesWithPaginationQuery request, CancellationToken cancellationToken)
        {
            PaginatedData<VisitorHistoryDto> data = await context.VisitorHistories.Where(x => x.Visitor.Name.Contains(request.Keyword) || x.Visitor.CompanyName.Contains(request.Keyword) || x.Comment.Contains(request.Keyword))
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<VisitorHistoryDto>(mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}