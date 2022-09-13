using CleanArchitecture.Blazor.Application.Features.VisitorHistories.DTOs;
using CleanArchitecture.Blazor.Application.Features.VisitorHistories.Caching;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.VisitorHistories.Queries.GetAll
{

    public class GetAllVisitorHistoriesQuery : IRequest<IEnumerable<VisitorHistoryDto>>, ICacheable
    {
        public string CacheKey => VisitorHistoryCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions? Options => VisitorHistoryCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllVisitorHistoriesQueryHandler :
         IRequestHandler<GetAllVisitorHistoriesQuery, IEnumerable<VisitorHistoryDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetAllVisitorHistoriesQueryHandler> localizer;

        public GetAllVisitorHistoriesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllVisitorHistoriesQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<IEnumerable<VisitorHistoryDto>> Handle(GetAllVisitorHistoriesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<VisitorHistoryDto> data = await context.VisitorHistories
                         .ProjectTo<VisitorHistoryDto>(mapper.ConfigurationProvider)
                         .ToListAsync(cancellationToken);
            return data;
        }
    }
}

