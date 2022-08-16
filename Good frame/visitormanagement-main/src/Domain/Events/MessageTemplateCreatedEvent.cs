namespace CleanArchitecture.Blazor.Domain.Events
{
    public class MessageTemplateCreatedEvent : DomainEvent
    {
        public MessageTemplateCreatedEvent(MessageTemplate item)
        {
            Item = item;
        }

        public MessageTemplate Item { get; }
    }
}

