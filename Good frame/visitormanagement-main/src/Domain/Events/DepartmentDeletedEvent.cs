namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DepartmentDeletedEvent : DomainEvent
    {
        public DepartmentDeletedEvent(Department item)
        {
            Item = item;
        }

        public Department Item { get; }
    }
}

