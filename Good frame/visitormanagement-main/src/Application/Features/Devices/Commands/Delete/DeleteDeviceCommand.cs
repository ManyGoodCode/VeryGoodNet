// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Devices.DTOs;
using CleanArchitecture.Blazor.Application.Features.Devices.Caching;
using MediatR;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using System.Threading;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Events;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Devices.Commands.Delete
{
    public class DeleteDeviceCommand : IRequest<Result>, ICacheInvalidator
    {
        public int[] Id { get; }
        public string CacheKey => DeviceCacheKey.GetAllCacheKey;
        public CancellationTokenSource? SharedExpiryTokenSource => DeviceCacheKey.SharedExpiryTokenSource();
        public DeleteDeviceCommand(int[] id)
        {
            Id = id;
        }
    }

    public class DeleteDeviceCommandHandler :
                 IRequestHandler<DeleteDeviceCommand, Result>

    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<DeleteDeviceCommandHandler> localizer;
        public DeleteDeviceCommandHandler(
            IApplicationDbContext context,
            IStringLocalizer<DeleteDeviceCommandHandler> localizer,
             IMapper mapper)
        {
            this.context = context;
            this.localizer = localizer;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
        {
            List<Device> items = await context.Devices.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
            foreach (Device item in items)
            {
                DeviceDeletedEvent deleteevent = new DeviceDeletedEvent(item);
                item.DomainEvents.Add(deleteevent);
                context.Devices.Remove(item);
            }

            await context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}

