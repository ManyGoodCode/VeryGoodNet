using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DepartmentUpdatedEvent : DomainEvent
    {
        public DepartmentUpdatedEvent(Department item)
        {
            Item = item;
        }

        public Department Item { get; }
    }
}
