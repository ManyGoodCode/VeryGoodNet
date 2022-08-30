using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Domain.Events
{
    public class EmployeeDeletedEvent : DomainEvent
    {
        public EmployeeDeletedEvent(Employee item)
        {
            Item = item;
        }

        public Employee Item { get; }
    }
}

