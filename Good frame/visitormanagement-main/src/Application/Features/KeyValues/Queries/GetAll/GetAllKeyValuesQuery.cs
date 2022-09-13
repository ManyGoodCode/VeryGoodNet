using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.ByName
{
    public class GetAllKeyValuesQuery : IRequest<IEnumerable<KeyValueDto>>, ICacheable
    {
        public string CacheKey => KeyValueCacheKey.GetAllCacheKey;

        public MemoryCacheEntryOptions? Options => KeyValueCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllKeyValuesQueryHandler : IRequestHandler<GetAllKeyValuesQuery, IEnumerable<KeyValueDto>>
    {

        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public GetAllKeyValuesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<KeyValueDto>> Handle(GetAllKeyValuesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<KeyValueDto> data = await context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
               .ProjectTo<KeyValueDto>(mapper.ConfigurationProvider)
               .ToListAsync(cancellationToken);
            return data;
        }
    }
}
