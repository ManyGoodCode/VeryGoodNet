using CleanArchitecture.Blazor.Domain.Common;
using MediatR;

namespace CleanArchitecture.Blazor.Application.Common.Models
{
    /// <summary>
    /// ��װ INotification ֪ͨ���ࡣ���ҽ���һ�� DomainEvent ����
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
