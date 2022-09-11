// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Departments.DTOs;
using CleanArchitecture.Blazor.Application.Features.Departments.Caching;
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

namespace CleanArchitecture.Blazor.Application.Features.Departments.Queries.GetAll
{

    public class GetAllDepartmentsQuery : IRequest<IEnumerable<DepartmentDto>>, ICacheable
    {
        public string CacheKey => DepartmentCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions? Options => DepartmentCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllDepartmentsQueryHandler :
         IRequestHandler<GetAllDepartmentsQuery, IEnumerable<DepartmentDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetAllDepartmentsQueryHandler> localizer;

        public GetAllDepartmentsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllDepartmentsQueryHandler> localizer )
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<IEnumerable<DepartmentDto>> Handle(GetAllDepartmentsQuery request, CancellationToken cancellationToken)
        {
            List<DepartmentDto> data = await context.Departments.OrderBy(x => x.Name)
                         .ProjectTo<DepartmentDto>(mapper.ConfigurationProvider)
                         .ToListAsync(cancellationToken);
            return data;
        }
    }
}


