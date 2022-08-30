using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class UpdatedEvent<T> : DomainEvent
    {
        public UpdatedEvent(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; }
    }
}
