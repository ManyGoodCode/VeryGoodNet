using CleanArchitecture.Blazor.Application.Features.Departments.DTOs;
using CleanArchitecture.Blazor.Application.Features.Departments.Caching;
using System.Threading.Tasks;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Models;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;

namespace CleanArchitecture.Blazor.Application.Features.Departments.Queries.Pagination
{

    public class DepartmentsWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<DepartmentDto>>, ICacheable
    {
        public string CacheKey => DepartmentCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => DepartmentCacheKey.MemoryCacheEntryOptions;
    }

    public class DepartmentsWithPaginationQueryHandler :
         IRequestHandler<DepartmentsWithPaginationQuery, PaginatedData<DepartmentDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;

        public DepartmentsWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<PaginatedData<DepartmentDto>> Handle(
            DepartmentsWithPaginationQuery request,
            CancellationToken cancellationToken)
        {
            PaginatedData<DepartmentDto> data = await context.Departments.Where(x => x.Name.Contains(request.Keyword))
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<DepartmentDto>(mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}