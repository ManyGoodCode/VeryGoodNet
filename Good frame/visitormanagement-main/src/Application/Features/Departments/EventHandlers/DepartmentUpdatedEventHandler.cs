// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Features.Departments.EventHandlers
{

    public class DepartmentUpdatedEventHandler : INotificationHandler<DomainEventNotification<DepartmentUpdatedEvent>>
    {
        private readonly ILogger<DepartmentUpdatedEventHandler> _logger;

        public DepartmentUpdatedEventHandler(
            ILogger<DepartmentUpdatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(DomainEventNotification<DepartmentUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);

            return Task.CompletedTask;
        }
    }
}