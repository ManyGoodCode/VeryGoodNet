namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DepartmentCreatedEvent : DomainEvent
    {
        public DepartmentCreatedEvent(Department item)
        {
            Item = item;
        }

        public Department Item { get; }
    }
}

