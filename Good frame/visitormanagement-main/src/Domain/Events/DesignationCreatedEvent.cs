namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DesignationCreatedEvent : DomainEvent
    {
        public DesignationCreatedEvent(Designation item)
        {
            Item = item;
        }

        public Designation Item { get; }
    }
}
