// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.EventHandlers
{

    public class CheckinPointUpdatedEventHandler : INotificationHandler<DomainEventNotification<CheckinPointUpdatedEvent>>
    {
        private readonly ILogger<CheckinPointUpdatedEventHandler> logger;

        public CheckinPointUpdatedEventHandler(
            ILogger<CheckinPointUpdatedEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(DomainEventNotification<CheckinPointUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            CheckinPointUpdatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
