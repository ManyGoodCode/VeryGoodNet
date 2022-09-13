using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Sites.EventHandlers
{

    public class SiteDeletedEventHandler : INotificationHandler<DomainEventNotification<SiteDeletedEvent>>
    {
        private readonly ILogger<SiteDeletedEventHandler> logger;

        public SiteDeletedEventHandler(
            ILogger<SiteDeletedEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(DomainEventNotification<SiteDeletedEvent> notification, CancellationToken cancellationToken)
        {
            SiteDeletedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
