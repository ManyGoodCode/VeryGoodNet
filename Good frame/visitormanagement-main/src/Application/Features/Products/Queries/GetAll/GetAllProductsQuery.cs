using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.GetAll
{

    public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>, ICacheable
    {
        public string CacheKey => ProductCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions? Options => ProductCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllProductsQueryHandler :
         IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetAllProductsQueryHandler> localizer;

        public GetAllProductsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllProductsQueryHandler> localizer)
        {
           this.context = context;
           this.mapper = mapper;
           this.localizer = localizer;
        }

        public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<ProductDto> data = await context.Products
                         .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
                         .ToListAsync(cancellationToken);
            return data;
        }
    }
}


