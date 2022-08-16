namespace CleanArchitecture.Blazor.Domain.Events
{
    public class VisitorHistoryDeletedEvent : DomainEvent
    {
        public VisitorHistoryDeletedEvent(VisitorHistory item)
        {
            Item = item;
        }

        public VisitorHistory Item { get; }
    }
}

