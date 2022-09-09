using CleanArchitecture.Blazor.Domain.Common;
using MediatR;

namespace CleanArchitecture.Blazor.Application.Common.Models
{
    /// <summary>
    /// 封装 INotification 通知的类。有且仅有一个 DomainEvent 属性
    /// </summary>
    public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
    {
        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }

        public TDomainEvent DomainEvent { get; }
    }
}
