using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

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
