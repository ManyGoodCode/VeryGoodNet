// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Designations.DTOs;
using CleanArchitecture.Blazor.Application.Features.Designations.Caching;
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

namespace CleanArchitecture.Blazor.Application.Features.Designations.Queries.GetAll
{
    public class GetAllDesignationsQuery : IRequest<IEnumerable<DesignationDto>>, ICacheable
    {
        public string CacheKey => DesignationCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions? Options => DesignationCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllDesignationsQueryHandler :
         IRequestHandler<GetAllDesignationsQuery, IEnumerable<DesignationDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetAllDesignationsQueryHandler> localizer;

        public GetAllDesignationsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllDesignationsQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<IEnumerable<DesignationDto>> Handle(
            GetAllDesignationsQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<DesignationDto> data = await context.Designations.OrderBy(x => x.Name)
                          .ProjectTo<DesignationDto>(mapper.ConfigurationProvider)
                          .ToListAsync(cancellationToken);
            return data;
        }
    }
}


