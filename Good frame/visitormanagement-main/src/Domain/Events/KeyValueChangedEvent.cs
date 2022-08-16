namespace CleanArchitecture.Blazor.Domain.Events
{
    public class KeyValueChangedEvent : DomainEvent
    {
        public KeyValueChangedEvent(KeyValue item)
        {
            Item = item;
        }

        public KeyValue Item { get; }
    }
}
