using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class VisitorHistoryCreatedEvent : DomainEvent
    {
        public VisitorHistoryCreatedEvent(VisitorHistory item)
        {
            Item = item;
        }

        public VisitorHistory Item { get; }
    }
}

