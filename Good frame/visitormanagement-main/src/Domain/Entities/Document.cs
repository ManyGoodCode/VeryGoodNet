using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities.Tenant;
namespace CleanArchitecture.Blazor.Domain.Entities
{
    public class Document : AuditableEntity, IHasDomainEvent
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public string? URL { get; set; }
        public int DocumentTypeId { get; set; }
        public virtual DocumentType DocumentType { get; set; } = default!;
        public System.Collections.Generic.List<DomainEvent> DomainEvents { get; set; } = new System.Collections.Generic.List<DomainEvent>();
    }
}
