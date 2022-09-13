// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.SiteConfigurations.DTOs;
using CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Caching;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Queries.GetAll
{
    public class GetBySiteIdConfigurationsQuery : IRequest<SiteConfigurationDto?>, ICacheable
    {
        public int? SiteId { get; private set; }
        public GetBySiteIdConfigurationsQuery(int? siteId)
        {
            SiteId = siteId;
        }
        public string CacheKey => SiteConfigurationCacheKey.GetBySiteIdCacheKey(SiteId);
        public MemoryCacheEntryOptions? Options => SiteConfigurationCacheKey.MemoryCacheEntryOptions;
    }

    public class GetBySiteIdConfigurationsQueryHandler :
         IRequestHandler<GetBySiteIdConfigurationsQuery, SiteConfigurationDto?>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetBySiteIdConfigurationsQueryHandler> localizer;

        public GetBySiteIdConfigurationsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetBySiteIdConfigurationsQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<SiteConfigurationDto?> Handle(
            GetBySiteIdConfigurationsQuery request,
            CancellationToken cancellationToken)
        {
            SiteConfigurationDto data = await context.SiteConfigurations.Where(x => x.SiteId == request.SiteId)
                         .ProjectTo<SiteConfigurationDto>(mapper.ConfigurationProvider)
                         .FirstOrDefaultAsync(cancellationToken);
            return data;
        }
    }
}


