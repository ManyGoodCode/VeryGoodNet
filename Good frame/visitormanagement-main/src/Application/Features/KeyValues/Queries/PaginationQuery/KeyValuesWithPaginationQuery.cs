using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using CleanArchitecture.Blazor.Application.Features.KeyValues.Caching;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.PaginationQuery
{
    public class KeyValuesWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<KeyValueDto>>, ICacheable
    {
        public string CacheKey => $"{nameof(KeyValuesWithPaginationQuery)},{this}";

        public MemoryCacheEntryOptions? Options => KeyValueCacheKey.MemoryCacheEntryOptions;
    }

    public class KeyValuesQueryHandler : IRequestHandler<KeyValuesWithPaginationQuery, PaginatedData<KeyValueDto>>
    {

        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public KeyValuesQueryHandler(

            IApplicationDbContext context,
            IMapper mapper)
        {

            this.context = context;
            this.mapper = mapper;
        }

        public async Task<PaginatedData<KeyValueDto>> Handle(KeyValuesWithPaginationQuery request, CancellationToken cancellationToken)
        {

            PaginatedData<KeyValueDto> data = await context.KeyValues.Where(x => x.Name.Contains(request.Keyword) || x.Value.Contains(request.Keyword) || x.Text.Contains(request.Keyword))
                //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectTo<KeyValueDto>(mapper.ConfigurationProvider)
                .PaginatedDataAsync(request.PageNumber, request.PageSize);

            return data;
        }
    }
}
