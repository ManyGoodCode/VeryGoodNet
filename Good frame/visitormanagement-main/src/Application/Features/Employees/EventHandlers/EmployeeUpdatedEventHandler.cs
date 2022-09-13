// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Employees.EventHandlers
{

    public class EmployeeUpdatedEventHandler : INotificationHandler<DomainEventNotification<EmployeeUpdatedEvent>>
    {
        private readonly ILogger<EmployeeUpdatedEventHandler> logger;

        public EmployeeUpdatedEventHandler(
            ILogger<EmployeeUpdatedEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(DomainEventNotification<EmployeeUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            EmployeeUpdatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
