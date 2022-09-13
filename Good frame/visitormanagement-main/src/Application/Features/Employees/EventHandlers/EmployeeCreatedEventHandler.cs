using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Employees.EventHandlers
{

    public class EmployeeCreatedEventHandler : INotificationHandler<DomainEventNotification<EmployeeCreatedEvent>>
    {
        private readonly ILogger<EmployeeCreatedEventHandler> logger;

        public EmployeeCreatedEventHandler(
            ILogger<EmployeeCreatedEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(DomainEventNotification<EmployeeCreatedEvent> notification, CancellationToken cancellationToken)
        {
            EmployeeCreatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
