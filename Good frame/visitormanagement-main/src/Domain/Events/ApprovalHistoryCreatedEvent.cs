namespace CleanArchitecture.Blazor.Domain.Events
{
    public class ApprovalHistoryCreatedEvent : DomainEvent
    {
        public ApprovalHistoryCreatedEvent(ApprovalHistory item)
        {
            Item = item;
        }

        public ApprovalHistory Item { get; }
    }
}

