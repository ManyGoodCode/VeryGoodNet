using CleanArchitecture.Blazor.Application.Features.Employees.DTOs;
using CleanArchitecture.Blazor.Application.Features.Employees.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Specification;
using CleanArchitecture.Blazor.Domain.Entities;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;

namespace CleanArchitecture.Blazor.Application.Features.Employees.Queries.Pagination
{

    public class EmployeesWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<EmployeeDto>>, ICacheable
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? DepartmentId { get; set; }
        public int? SiteId { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()},Name:{Name},Email:{Email},DepartmentId:{DepartmentId}";
        }

        public string CacheKey => EmployeeCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => EmployeeCacheKey.MemoryCacheEntryOptions;
    }

    public class EmployeesWithPaginationQueryHandler :
         IRequestHandler<EmployeesWithPaginationQuery, PaginatedData<EmployeeDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<EmployeesWithPaginationQueryHandler> localizer;

        public EmployeesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<EmployeesWithPaginationQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<PaginatedData<EmployeeDto>> Handle(EmployeesWithPaginationQuery request, CancellationToken cancellationToken)
        {
            PaginatedData<EmployeeDto> data = await context.Employees.Specify(new SearchEmployeeSpecification(request))
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<EmployeeDto>(mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }

    public class SearchEmployeeSpecification : Specification<Employee>
    {
        public SearchEmployeeSpecification(EmployeesWithPaginationQuery query)
        {
            AddInclude(x => x.Department);
            AddInclude(x => x.Designation);
            AddInclude(x => x.Site);
            Criteria = q => q.Name != null;
            if (!string.IsNullOrEmpty(query.Keyword))
            {
                And(x => x.Name.Contains(query.Keyword) || x.About.Contains(query.Keyword) || x.PhoneNumber.Contains(query.Keyword));
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                And(x => x.Name.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Email))
            {
                And(x => x.Name.Contains(query.Email));
            }
            if (query.DepartmentId != null)
            {
                And(x => x.DepartmentId == query.DepartmentId);
            }
        }
    }
}