using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities.Tenant;

namespace CleanArchitecture.Blazor.Domain.Entities
{
    public class Department : AuditableEntity, IHasDomainEvent, IAuditTrial
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }

    public class Designation : AuditableEntity, IHasDomainEvent, IAuditTrial
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
    }
}