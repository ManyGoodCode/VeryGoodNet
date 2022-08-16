namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DeviceCreatedEvent : DomainEvent
    {
        public DeviceCreatedEvent(Device item)
        {
            Item = item;
        }

        public Device Item { get; }
    }
}

