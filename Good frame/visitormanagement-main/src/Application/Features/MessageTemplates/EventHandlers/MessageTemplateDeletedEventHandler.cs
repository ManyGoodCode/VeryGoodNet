using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.MessageTemplates.EventHandlers
{

    public class MessageTemplateDeletedEventHandler : INotificationHandler<DomainEventNotification<DeletedEvent<MessageTemplate>>>
    {
        private readonly ILogger<MessageTemplateDeletedEventHandler> logger;

        public MessageTemplateDeletedEventHandler(
            ILogger<MessageTemplateDeletedEventHandler> logger)
        {
           this.logger = logger;
        }

        public Task Handle(DomainEventNotification<DeletedEvent<MessageTemplate>> notification, CancellationToken cancellationToken)
        {
            DeletedEvent<MessageTemplate> domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
