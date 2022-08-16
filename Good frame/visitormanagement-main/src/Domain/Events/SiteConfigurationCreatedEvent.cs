namespace CleanArchitecture.Blazor.Domain.Events
{
    public class SiteConfigurationCreatedEvent : DomainEvent
    {
        public SiteConfigurationCreatedEvent(SiteConfiguration item)
        {
            Item = item;
        }

        public SiteConfiguration Item { get; }
    }
}

