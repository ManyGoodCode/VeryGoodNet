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

    public class CheckinPointDeletedEventHandler : INotificationHandler<DomainEventNotification<CheckinPointDeletedEvent>>
    {
        private readonly ILogger<CheckinPointDeletedEventHandler> logger;

        public CheckinPointDeletedEventHandler(
            ILogger<CheckinPointDeletedEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(DomainEventNotification<CheckinPointDeletedEvent> notification, CancellationToken cancellationToken)
        {
            CheckinPointDeletedEvent domainEvent = notification.DomainEvent;
            this.logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
