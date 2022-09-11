using CleanArchitecture.Blazor.Application.Features.CheckinPoints.DTOs;
using CleanArchitecture.Blazor.Application.Features.CheckinPoints.Caching;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;
using AutoMapper;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.Queries.Pagination
{

    public class CheckinPointsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<CheckinPointDto>>, ICacheable
    {
        public string CacheKey => CheckinPointCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => CheckinPointCacheKey.MemoryCacheEntryOptions;
    }

    public class CheckinPointsWithPaginationQueryHandler :
         IRequestHandler<CheckinPointsWithPaginationQuery, PaginatedData<CheckinPointDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<CheckinPointsWithPaginationQueryHandler> localizer;

        public CheckinPointsWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<CheckinPointsWithPaginationQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<PaginatedData<CheckinPointDto>> Handle(CheckinPointsWithPaginationQuery request, CancellationToken cancellationToken)
        {

            PaginatedData<CheckinPointDto> data = await context.CheckinPoints.Where(x => x.Name.Contains(request.Keyword) || x.Description.Contains(request.Keyword))
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<CheckinPointDto>(mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }

}