// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Application.Features.Devices.DTOs;
using CleanArchitecture.Blazor.Application.Features.Devices.Caching;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Domain.Events;

namespace CleanArchitecture.Blazor.Application.Features.Devices.Commands.AddEdit
{
    public class AddEditDeviceCommand : DeviceDto, IRequest<Result<int>>, IMapFrom<Device>, ICacheInvalidator
    {
        public string CacheKey => DeviceCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => DeviceCacheKey.SharedExpiryTokenSource();
    }

    public class AddEditDeviceCommandHandler : IRequestHandler<AddEditDeviceCommand, Result<int>>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<AddEditDeviceCommandHandler> localizer;
        public AddEditDeviceCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<AddEditDeviceCommandHandler> localizer,
            IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }
        public async Task<Result<int>> Handle(AddEditDeviceCommand request, CancellationToken cancellationToken)
        {

            if (request.Id > 0)
            {
                Device item = await context.Devices.FindAsync(new object[] { request.Id }, cancellationToken);
                _ = item ?? throw new NotFoundException($"Device {request.Id} Not Found.");
                item = mapper.Map(request, item);
                DeviceUpdatedEvent updateevent = new DeviceUpdatedEvent(item);
                item.DomainEvents.Add(updateevent);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
            else
            {
                Device item = mapper.Map<Device>(request);
                DeviceCreatedEvent careateevent = new DeviceCreatedEvent(item);
                item.DomainEvents.Add(careateevent);
                context.Devices.Add(item);
                await context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(item.Id);
            }
        }
    }
}

