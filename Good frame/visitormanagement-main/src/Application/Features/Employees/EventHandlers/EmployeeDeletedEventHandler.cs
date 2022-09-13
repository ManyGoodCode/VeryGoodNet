using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Employees.EventHandlers
{

    public class EmployeeDeletedEventHandler : INotificationHandler<DomainEventNotification<EmployeeDeletedEvent>>
    {
        private readonly ILogger<EmployeeDeletedEventHandler> logger;

        public EmployeeDeletedEventHandler(
            ILogger<EmployeeDeletedEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(
            DomainEventNotification<EmployeeDeletedEvent> notification,
            CancellationToken cancellationToken)
        {
            EmployeeDeletedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
