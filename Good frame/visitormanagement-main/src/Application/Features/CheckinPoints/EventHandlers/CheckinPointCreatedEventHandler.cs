// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.EventHandlers
{

    public class CheckinPointCreatedEventHandler : INotificationHandler<DomainEventNotification<CheckinPointCreatedEvent>>
    {
        private readonly ILogger<CheckinPointCreatedEventHandler> _logger;

        public CheckinPointCreatedEventHandler(
            ILogger<CheckinPointCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(DomainEventNotification<CheckinPointCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);

            return Task.CompletedTask;
        }
    }
}
