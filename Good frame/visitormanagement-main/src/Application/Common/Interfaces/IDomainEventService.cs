using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{
    /// <summary>
    /// 发布 Domain层事件
    /// </summary>
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
