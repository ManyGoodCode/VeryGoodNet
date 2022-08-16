namespace CleanArchitecture.Blazor.Domain.Events
{
    public class CheckinPointDeletedEvent : DomainEvent
    {
        public CheckinPointDeletedEvent(CheckinPoint item)
        {
            Item = item;
        }

        public CheckinPoint Item { get; }
    }
}

