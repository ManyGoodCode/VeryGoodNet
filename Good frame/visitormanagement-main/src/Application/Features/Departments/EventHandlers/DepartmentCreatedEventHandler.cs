// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Departments.EventHandlers
{

    public class DepartmentCreatedEventHandler : INotificationHandler<DomainEventNotification<DepartmentCreatedEvent>>
    {
        private readonly ILogger<DepartmentCreatedEventHandler> logger;

        public DepartmentCreatedEventHandler(
            ILogger<DepartmentCreatedEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(DomainEventNotification<DepartmentCreatedEvent> notification, CancellationToken cancellationToken)
        {
            DepartmentCreatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}