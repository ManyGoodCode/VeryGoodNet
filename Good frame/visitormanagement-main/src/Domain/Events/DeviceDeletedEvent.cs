namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DeviceDeletedEvent : DomainEvent
    {
        public DeviceDeletedEvent(Device item)
        {
            Item = item;
        }

        public Device Item { get; }
    }
}

