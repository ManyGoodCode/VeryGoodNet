namespace CleanArchitecture.Blazor.Domain.Events
{
    public class CheckinPointUpdatedEvent : DomainEvent
    {
        public CheckinPointUpdatedEvent(CheckinPoint item)
        {
            Item = item;
        }

        public CheckinPoint Item { get; }
    }
}

