// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Devices.EventHandlers
{

    public class DeviceDeletedEventHandler : INotificationHandler<DomainEventNotification<DeviceDeletedEvent>>
    {
        private readonly ILogger<DeviceDeletedEventHandler> logger;

        public DeviceDeletedEventHandler(
            ILogger<DeviceDeletedEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(
            DomainEventNotification<DeviceDeletedEvent> notification,
            CancellationToken cancellationToken)
        {
            DeviceDeletedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
