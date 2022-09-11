using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Designations.EventHandlers
{

    public class DesignationCreatedEventHandler : INotificationHandler<DomainEventNotification<DesignationCreatedEvent>>
    {
        private readonly ILogger<DesignationCreatedEventHandler> logger;

        public DesignationCreatedEventHandler(
            ILogger<DesignationCreatedEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(DomainEventNotification<DesignationCreatedEvent> notification, CancellationToken cancellationToken)
        {
            DesignationCreatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}