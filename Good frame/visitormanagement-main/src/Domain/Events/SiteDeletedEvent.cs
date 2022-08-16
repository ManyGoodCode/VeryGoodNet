namespace CleanArchitecture.Blazor.Domain.Events
{
    public class SiteDeletedEvent : DomainEvent
    {
        public SiteDeletedEvent(Site item)
        {
            Item = item;
        }

        public Site Item { get; }
    }
}

