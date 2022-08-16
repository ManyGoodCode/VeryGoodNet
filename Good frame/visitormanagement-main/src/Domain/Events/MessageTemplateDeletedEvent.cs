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

