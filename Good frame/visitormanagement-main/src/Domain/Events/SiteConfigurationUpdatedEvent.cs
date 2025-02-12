using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class SiteConfigurationUpdatedEvent : DomainEvent
    {
        public SiteConfigurationUpdatedEvent(SiteConfiguration item)
        {
            Item = item;
        }

        public SiteConfiguration Item { get; }
    }
}

