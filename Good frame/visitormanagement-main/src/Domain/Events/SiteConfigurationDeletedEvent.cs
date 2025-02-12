using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class SiteConfigurationDeletedEvent : DomainEvent
    {
        public SiteConfigurationDeletedEvent(SiteConfiguration item)
        {
            Item = item;
        }

        public SiteConfiguration Item { get; }
    }
}

