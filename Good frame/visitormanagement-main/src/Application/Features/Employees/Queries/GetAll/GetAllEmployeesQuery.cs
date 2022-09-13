using CleanArchitecture.Blazor.Application.Features.Employees.DTOs;
using CleanArchitecture.Blazor.Application.Features.Employees.Caching;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.Employees.Queries.GetAll
{

    public class GetAllEmployeesQuery : IRequest<IEnumerable<EmployeeDto>>, ICacheable
    {
        public string CacheKey => EmployeeCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions? Options => EmployeeCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllEmployeesQueryHandler :
         IRequestHandler<GetAllEmployeesQuery, IEnumerable<EmployeeDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<GetAllEmployeesQueryHandler> localizer;

        public GetAllEmployeesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllEmployeesQueryHandler> localizer
            )
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<IEnumerable<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<EmployeeDto> data = await context.Employees.OrderBy(x => x.Name)
                         .ProjectTo<EmployeeDto>(mapper.ConfigurationProvider)
                         .ToListAsync(cancellationToken);
            return data;
        }
    }
}

