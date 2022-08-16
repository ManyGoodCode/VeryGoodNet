namespace CleanArchitecture.Blazor.Domain.Entities
{
    public class KeyValue : AuditableEntity, IHasDomainEvent, IAuditTrial
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
        public List<DomainEvent> DomainEvents { get; set; } = new();
    }
}
