using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Sites.EventHandlers
{

    public class SiteUpdatedEventHandler : INotificationHandler<DomainEventNotification<SiteUpdatedEvent>>
    {
        private readonly ILogger<SiteUpdatedEventHandler> logger;

        public SiteUpdatedEventHandler(
            ILogger<SiteUpdatedEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(DomainEventNotification<SiteUpdatedEvent> notification, CancellationToken cancellationToken)
        {
            SiteUpdatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
