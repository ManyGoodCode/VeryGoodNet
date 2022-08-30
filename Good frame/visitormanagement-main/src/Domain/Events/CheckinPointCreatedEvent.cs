using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class CheckinPointCreatedEvent : DomainEvent
    {
        public CheckinPointCreatedEvent(CheckinPoint item)
        {
            Item = item;
        }

        public CheckinPoint Item { get; }
    }
}

