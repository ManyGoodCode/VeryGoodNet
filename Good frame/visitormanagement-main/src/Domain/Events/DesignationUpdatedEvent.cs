namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DesignationUpdatedEvent : DomainEvent
    {
        public DesignationUpdatedEvent(Designation item)
        {
            Item = item;
        }

        public Designation Item { get; }
    }
}

