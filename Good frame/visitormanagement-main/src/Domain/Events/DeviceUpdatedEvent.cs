namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DeviceUpdatedEvent : DomainEvent
    {
        public DeviceUpdatedEvent(Device item)
        {
            Item = item;
        }

        public Device Item { get; }
    }
}

