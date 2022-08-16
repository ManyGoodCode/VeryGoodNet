namespace CleanArchitecture.Blazor.Domain.Events
{
    public class SiteUpdatedEvent : DomainEvent
    {
        public SiteUpdatedEvent(Site item)
        {
            Item = item;
        }

        public Site Item { get; }
    }
}

