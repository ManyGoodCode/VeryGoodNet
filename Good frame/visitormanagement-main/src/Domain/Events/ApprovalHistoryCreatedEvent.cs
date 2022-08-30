using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;
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

