// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Devices.DTOs;
using CleanArchitecture.Blazor.Application.Features.Devices.Caching;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace CleanArchitecture.Blazor.Application.Features.Devices.Queries.GetAll
{

    public class GetAllDevicesQuery : IRequest<IEnumerable<DeviceDto>>, ICacheable
    {
        public string CacheKey => DeviceCacheKey.GetAllCacheKey;
        public MemoryCacheEntryOptions? Options => DeviceCacheKey.MemoryCacheEntryOptions;
    }

    public class GetAllDevicesQueryHandler :
         IRequestHandler<GetAllDevicesQuery, IEnumerable<DeviceDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetAllDevicesQueryHandler> _localizer;

        public GetAllDevicesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IStringLocalizer<GetAllDevicesQueryHandler> localizer
            )
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<IEnumerable<DeviceDto>> Handle(GetAllDevicesQuery request, CancellationToken cancellationToken)
        {

            var data = await _context.Devices.OrderBy(x => x.Name)
                         .ProjectTo<DeviceDto>(_mapper.ConfigurationProvider)
                         .ToListAsync(cancellationToken);
            return data;
        }
    }
}


