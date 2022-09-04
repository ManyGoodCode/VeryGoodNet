// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.SiteConfigurations.DTOs;
using CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Caching;
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
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Queries.Pagination
{

    public class SiteConfigurationsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<SiteConfigurationDto>>, ICacheable
    {
        public string CacheKey => SiteConfigurationCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => SiteConfigurationCacheKey.MemoryCacheEntryOptions;
    }

    public class SiteConfigurationsWithPaginationQueryHandler :
         IRequestHandler<SiteConfigurationsWithPaginationQuery, PaginatedData<SiteConfigurationDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SiteConfigurationsWithPaginationQueryHandler> _localizer;

        public SiteConfigurationsWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<SiteConfigurationsWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<SiteConfigurationDto>> Handle(SiteConfigurationsWithPaginationQuery request, CancellationToken cancellationToken)
        {

            var data = await _context.SiteConfigurations.Where(x => x.Site.Name.Contains(request.Keyword))
                  .Include(x => x.Site)
                 .OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<SiteConfigurationDto>(_mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}