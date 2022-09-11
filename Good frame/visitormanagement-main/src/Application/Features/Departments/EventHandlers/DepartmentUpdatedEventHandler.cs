
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Departments.EventHandlers
{

    public class DepartmentUpdatedEventHandler : INotificationHandler<DomainEventNotification<DepartmentUpdatedEvent>>
    {
        private readonly ILogger<DepartmentUpdatedEventHandler> logger;

        public DepartmentUpdatedEventHandler(
            ILogger<DepartmentUpdatedEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(DomainEventNotification<DepartmentUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            DepartmentUpdatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}