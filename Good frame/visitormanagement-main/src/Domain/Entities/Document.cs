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
        public List<DomainEvent> DomainEvents { get; set; } = new();
    }
}
