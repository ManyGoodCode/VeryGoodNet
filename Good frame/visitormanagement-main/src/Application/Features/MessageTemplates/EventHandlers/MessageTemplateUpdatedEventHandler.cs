using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.MessageTemplates.EventHandlers
{

    public class MessageTemplateUpdatedEventHandler : INotificationHandler<DomainEventNotification<UpdatedEvent<MessageTemplate>>>
    {
        private readonly ILogger<MessageTemplateUpdatedEventHandler> logger;

        public MessageTemplateUpdatedEventHandler(
            ILogger<MessageTemplateUpdatedEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(DomainEventNotification<UpdatedEvent<MessageTemplate>> notification, CancellationToken cancellationToken)
        {
            UpdatedEvent<MessageTemplate> domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
