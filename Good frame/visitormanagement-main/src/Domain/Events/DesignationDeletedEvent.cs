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
