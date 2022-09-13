using CleanArchitecture.Blazor.Application.Features.SiteConfigurations.DTOs;
using CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Caching;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Queries.GetAll
{
    public class GetAllSiteConfigurationsQuery : IRequest<IEnumerable<SiteConfigurationDto>>, ICacheable
    {
        public string CacheKey => SiteConfigurationCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions? Options => SiteConfigurationCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllSiteConfigurationsQueryHandler :
         IRequestHandler<GetAllSiteConfigurationsQuery, IEnumerable<SiteConfigurationDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetAllSiteConfigurationsQueryHandler> localizer;

        public GetAllSiteConfigurationsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllSiteConfigurationsQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<IEnumerable<SiteConfigurationDto>> Handle(GetAllSiteConfigurationsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<SiteConfigurationDto> data = await context.SiteConfigurations.OrderBy(x => x.SiteId)
                         .ProjectTo<SiteConfigurationDto>(mapper.ConfigurationProvider)
                         .ToListAsync(cancellationToken);
            return data;
        }
    }
}


