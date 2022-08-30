using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class CreatedEvent<T> : DomainEvent
    {
        public CreatedEvent(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
