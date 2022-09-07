using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{

    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
