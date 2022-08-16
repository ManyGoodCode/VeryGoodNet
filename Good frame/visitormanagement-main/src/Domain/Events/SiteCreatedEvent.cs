namespace CleanArchitecture.Blazor.Domain.Events
{
    public class SiteCreatedEvent : DomainEvent
    {
        public SiteCreatedEvent(Site item)
        {
            Item = item;
        }

        public Site Item { get; }
    }
}

