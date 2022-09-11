using CleanArchitecture.Blazor.Application.Features.CheckinPoints.DTOs;
using CleanArchitecture.Blazor.Application.Features.CheckinPoints.Caching;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using Microsoft.Extensions.Localization;
using AutoMapper;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.Queries.GetAll
{

    public class GetAllCheckinPointsQuery : IRequest<IEnumerable<CheckinPointDto>>, ICacheable
    {
        public string CacheKey => CheckinPointCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions? Options => CheckinPointCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllCheckinPointsQueryHandler :
         IRequestHandler<GetAllCheckinPointsQuery, IEnumerable<CheckinPointDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetAllCheckinPointsQueryHandler> localizer;

        public GetAllCheckinPointsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllCheckinPointsQueryHandler> localizer
            )
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<IEnumerable<CheckinPointDto>> Handle(
            GetAllCheckinPointsQuery request,
            CancellationToken cancellationToken)
        {

            IEnumerable<CheckinPointDto> data = await context.CheckinPoints.OrderBy(x => x.Name)
                         .ProjectTo<CheckinPointDto>(mapper.ConfigurationProvider)
                         .ToListAsync(cancellationToken);
            return data;
        }
    }
}


