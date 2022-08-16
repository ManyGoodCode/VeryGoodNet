namespace CleanArchitecture.Blazor.Domain.Events
{
    public class MessageTemplateUpdatedEvent : DomainEvent
    {
        public MessageTemplateUpdatedEvent(MessageTemplate item)
        {
            Item = item;
        }

        public MessageTemplate Item { get; }
    }
}
