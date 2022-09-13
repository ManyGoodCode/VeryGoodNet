// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName
{
    public class KeyValuesQueryByName : IRequest<IEnumerable<KeyValueDto>>, ICacheable
    {
        public string Name { get; set; }

        public string CacheKey => KeyValueCacheKey.GetCacheKey(Name);

        public MemoryCacheEntryOptions? Options => KeyValueCacheKey.MemoryCacheEntryOptions;
        public KeyValuesQueryByName(string name)
        {
            Name = name;
        }
    }

    public class KeyValuesQueryByNameHandler : IRequestHandler<KeyValuesQueryByName, IEnumerable<KeyValueDto>>
    {

        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public KeyValuesQueryByNameHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<KeyValueDto>> Handle(KeyValuesQueryByName request, CancellationToken cancellationToken)
        {
            IEnumerable<KeyValueDto> data = await context.KeyValues.Where(x => x.Name == request.Name)
                .OrderBy(x => x.Text)
               .ProjectTo<KeyValueDto>(mapper.ConfigurationProvider)
               .ToListAsync(cancellationToken);
            return data;
        }
    }
}
