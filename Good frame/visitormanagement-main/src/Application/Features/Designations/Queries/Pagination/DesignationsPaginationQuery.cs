// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Designations.DTOs;
using CleanArchitecture.Blazor.Application.Features.Designations.Caching;
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

namespace CleanArchitecture.Blazor.Application.Features.Designations.Queries.Pagination
{
    public class DesignationsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<DesignationDto>>, ICacheable
    {
        public string CacheKey => DesignationCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => DesignationCacheKey.MemoryCacheEntryOptions;
    }

    public class DesignationsWithPaginationQueryHandler :
         IRequestHandler<DesignationsWithPaginationQuery, PaginatedData<DesignationDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DesignationsWithPaginationQueryHandler> localizer;

        public DesignationsWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<DesignationsWithPaginationQueryHandler> localizer
            )
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<PaginatedData<DesignationDto>> Handle(DesignationsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            PaginatedData<DesignationDto> data = await context.Designations
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<DesignationDto>(mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}