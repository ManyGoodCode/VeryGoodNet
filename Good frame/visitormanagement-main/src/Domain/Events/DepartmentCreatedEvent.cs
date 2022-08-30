using CleanArchitecture.Blazor.Domain.Common;
namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DepartmentCreatedEvent : DomainEvent
    {
        public DepartmentCreatedEvent(Entities.Department item)
        {
            Item = item;
        }

        public Entities.Department Item { get; }
    }
}

