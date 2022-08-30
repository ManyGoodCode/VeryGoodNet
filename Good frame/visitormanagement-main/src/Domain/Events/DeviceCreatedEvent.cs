using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

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

