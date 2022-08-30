using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class MessageTemplateDeletedEvent : DomainEvent
    {
        public MessageTemplateDeletedEvent(MessageTemplate item)
        {
            Item = item;
        }

        public MessageTemplate Item { get; }
    }
}

