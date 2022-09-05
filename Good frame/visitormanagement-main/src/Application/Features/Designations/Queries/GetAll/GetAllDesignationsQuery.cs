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
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetAllDesignationsQueryHandler> _localizer;

        public GetAllDesignationsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllDesignationsQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<IEnumerable<DesignationDto>> Handle(GetAllDesignationsQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Designations.OrderBy(x => x.Name)
                          .ProjectTo<DesignationDto>(_mapper.ConfigurationProvider)
                          .ToListAsync(cancellationToken);
            return data;
        }
    }
}


