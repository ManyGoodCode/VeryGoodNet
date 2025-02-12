using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class DeletedEvent<T> : DomainEvent
    {
        public DeletedEvent(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
