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

    public class DeviceCreatedEventHandler : INotificationHandler<DomainEventNotification<DeviceCreatedEvent>>
    {
        private readonly ILogger<DeviceCreatedEventHandler> _logger;

        public DeviceCreatedEventHandler(
            ILogger<DeviceCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(DomainEventNotification<DeviceCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);

            return Task.CompletedTask;
        }
    }
}
