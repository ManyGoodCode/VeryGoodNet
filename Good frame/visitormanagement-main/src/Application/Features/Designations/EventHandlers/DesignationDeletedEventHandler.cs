using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Designations.EventHandlers
{

    public class DesignationDeletedEventHandler : INotificationHandler<DomainEventNotification<DesignationDeletedEvent>>
    {
        private readonly ILogger<DesignationDeletedEventHandler> logger;

        public DesignationDeletedEventHandler(
            ILogger<DesignationDeletedEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(DomainEventNotification<DesignationDeletedEvent> notification, CancellationToken cancellationToken)
        {
            DesignationDeletedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
