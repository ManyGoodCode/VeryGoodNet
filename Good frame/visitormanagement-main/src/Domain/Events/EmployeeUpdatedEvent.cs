namespace CleanArchitecture.Blazor.Domain.Events
{
    public class EmployeeUpdatedEvent : DomainEvent
    {
        public EmployeeUpdatedEvent(Employee item)
        {
            Item = item;
        }

        public Employee Item { get; }
    }
}

