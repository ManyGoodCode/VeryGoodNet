using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.EventHandlers
{

    public class KeyValueChangedEventHandler : INotificationHandler<DomainEventNotification<KeyValueChangedEvent>>
    {
        private readonly IPicklistService picklistService;
        private readonly ILogger<KeyValueChangedEventHandler> logger;

        public KeyValueChangedEventHandler(
            IPicklistService picklistService,
            ILogger<KeyValueChangedEventHandler> logger)
        {
            this.picklistService = picklistService;
            this.logger = logger;
        }

        public async Task Handle(
            DomainEventNotification<KeyValueChangedEvent> notification,
            CancellationToken cancellationToken)
        {
            KeyValueChangedEvent domainEvent = notification.DomainEvent;
            logger.LogInformation("KeyValue Changed {DomainEvent}", domainEvent.GetType().Name);
            await picklistService.Refresh();
        }
    }
}
