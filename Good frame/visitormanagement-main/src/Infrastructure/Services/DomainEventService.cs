using System;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Infrastructure.Services
{
    public class DomainEventService : IDomainEventService
    {
        private readonly ILogger<DomainEventService> logger;
        private readonly MediatR.IPublisher mediator;

        public DomainEventService(ILogger<DomainEventService> logger, IPublisher mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }

        public async Task Publish(DomainEvent domainEvent)
        {
            logger.LogInformation("Publishing domain event. Event - {event}", nameof(domainEvent));
            await mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
        }

        private INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
        {
            return (INotification)Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent)!;
        }
    }
}
