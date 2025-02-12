using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Sites.EventHandlers
{

    public class SiteCreatedEventHandler : INotificationHandler<DomainEventNotification<SiteCreatedEvent>>
    {
        private readonly ILogger<SiteCreatedEventHandler> logger;

        public SiteCreatedEventHandler(
            ILogger<SiteCreatedEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(DomainEventNotification<SiteCreatedEvent> notification, CancellationToken cancellationToken)
        {
            SiteCreatedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
