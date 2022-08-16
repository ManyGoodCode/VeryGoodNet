namespace CleanArchitecture.Blazor.Domain.Events
{
    public class ApprovalHistoryUpdatedEvent : DomainEvent
    {
        public ApprovalHistoryUpdatedEvent(ApprovalHistory item)
        {
            Item = item;
        }

        public ApprovalHistory Item { get; }
    }
}

