using CleanArchitecture.Blazor.Application.Features.Sites.DTOs;
using CleanArchitecture.Blazor.Application.Features.Sites.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;

namespace CleanArchitecture.Blazor.Application.Features.Sites.Queries.Pagination
{
    public class SitesWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<SiteDto>>, ICacheable
    {
        public string CacheKey => SiteCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => SiteCacheKey.MemoryCacheEntryOptions;
    }

    public class SitesWithPaginationQueryHandler :
         IRequestHandler<SitesWithPaginationQuery, PaginatedData<SiteDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<SitesWithPaginationQueryHandler> localizer;

        public SitesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<SitesWithPaginationQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<PaginatedData<SiteDto>> Handle(SitesWithPaginationQuery request, CancellationToken cancellationToken)
        {
            PaginatedData<SiteDto> data = await context.Sites.Where(x => x.Name.Contains(request.Keyword) || x.Address.Contains(request.Keyword))
                 .Include(x => x.CheckinPoints)
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<SiteDto>(mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}