// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

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
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SitesWithPaginationQueryHandler> _localizer;

        public SitesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<SitesWithPaginationQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<PaginatedData<SiteDto>> Handle(SitesWithPaginationQuery request, CancellationToken cancellationToken)
        {
            //var result=await _context.Sites.Include(x=>x.CheckinPoints).ToListAsync(cancellationToken);

            var data = await _context.Sites.Where(x => x.Name.Contains(request.Keyword) || x.Address.Contains(request.Keyword))
                 .Include(x => x.CheckinPoints)
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<SiteDto>(_mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}