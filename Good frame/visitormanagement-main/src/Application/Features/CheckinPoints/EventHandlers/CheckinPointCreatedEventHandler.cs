using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.EventHandlers
{

    public class CheckinPointCreatedEventHandler : INotificationHandler<DomainEventNotification<CheckinPointCreatedEvent>>
    {
        private readonly ILogger<CheckinPointCreatedEventHandler> logger;

        public CheckinPointCreatedEventHandler(
            ILogger<CheckinPointCreatedEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(DomainEventNotification<CheckinPointCreatedEvent> notification, CancellationToken cancellationToken)
        {
            CheckinPointCreatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
