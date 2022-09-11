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

    public class DeviceUpdatedEventHandler : INotificationHandler<DomainEventNotification<DeviceUpdatedEvent>>
    {
        private readonly ILogger<DeviceUpdatedEventHandler> logger;

        public DeviceUpdatedEventHandler(
            ILogger<DeviceUpdatedEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(DomainEventNotification<DeviceUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            DeviceUpdatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
