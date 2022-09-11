using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Designations.EventHandlers
{

    public class DesignationUpdatedEventHandler : INotificationHandler<DomainEventNotification<DesignationUpdatedEvent>>
    {
        private readonly ILogger<DesignationUpdatedEventHandler> logger;

        public DesignationUpdatedEventHandler(
            ILogger<DesignationUpdatedEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(DomainEventNotification<DesignationUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            DesignationUpdatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
