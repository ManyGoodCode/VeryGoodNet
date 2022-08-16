namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DocumentCreatedEvent : DomainEvent
    {
        public DocumentCreatedEvent(Document item)
        {
            Item = item;
        }

        public Document Item { get; }
    }
}
