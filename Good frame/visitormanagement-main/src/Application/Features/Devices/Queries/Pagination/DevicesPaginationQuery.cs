using CleanArchitecture.Blazor.Application.Features.Devices.DTOs;
using CleanArchitecture.Blazor.Application.Features.Devices.Caching;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using System.Threading;
using Microsoft.Extensions.Localization;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using System.Linq;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Devices.Queries.Pagination
{

    public class DevicesWithPaginationQuery : PaginationFilter, IRequest<PaginatedData<DeviceDto>>, ICacheable
    {
        public string CacheKey => DeviceCacheKey.GetPagtionCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => DeviceCacheKey.MemoryCacheEntryOptions;
    }

    public class DevicesWithPaginationQueryHandler :
         IRequestHandler<DevicesWithPaginationQuery, PaginatedData<DeviceDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DevicesWithPaginationQueryHandler> localizer;

        public DevicesWithPaginationQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<DevicesWithPaginationQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.localizer = localizer;
        }

        public async Task<PaginatedData<DeviceDto>> Handle(
            DevicesWithPaginationQuery request,
            CancellationToken cancellationToken)
        {
            PaginatedData<DeviceDto> data = await context.Devices.Where(x => x.Name.Contains(request.Keyword))
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<DeviceDto>(mapper.ConfigurationProvider)
                 .PaginatedDataAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}