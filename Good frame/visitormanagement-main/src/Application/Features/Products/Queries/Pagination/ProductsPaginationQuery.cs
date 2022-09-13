using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.Products.Caching;
using CleanArchitecture.Blazor.Application.Features.Products.DTOs;
using CleanArchitecture.Blazor.Application.Features.Products.Queries.Specification;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Application.Features.Products.Queries.Pagination
{
    public class ProductsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<ProductDto>>, ICacheable
    {
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? Unit { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public override string ToString()
        {
            return $"{base.ToString()},Name:{Name},Brand:{Brand},Unit:{Unit},MinPrice:{MinPrice},MaxPrice:{MaxPrice}";
        }

        public string CacheKey => ProductCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => ProductCacheKey.MemoryCacheEntryOptions;
    }

    public class ProductsWithPaginationQueryHandler :
             IRequestHandler<ProductsWithPaginationQuery, PaginatedData<ProductDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<ProductsWithPaginationQueryHandler> localizer;

        public ProductsWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<ProductsWithPaginationQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<PaginatedData<ProductDto>> Handle(ProductsWithPaginationQuery request, CancellationToken cancellationToken)
        {
            PaginatedData<ProductDto> data = await context.Products.Specify(new SearchProductSpecification(request))
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<ProductDto>(mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}