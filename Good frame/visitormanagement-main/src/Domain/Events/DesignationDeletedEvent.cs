using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DesignationDeletedEvent : DomainEvent
    {
        public DesignationDeletedEvent(Designation item)
        {
            Item = item;
        }

        public Designation Item { get; }
    }
}
