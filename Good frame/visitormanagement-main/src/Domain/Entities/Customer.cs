using CleanArchitecture.Blazor.Domain.Common;
namespace CleanArchitecture.Blazor.Domain.Entities
{
    public partial class Customer : AuditableEntity, IHasDomainEvent, IAuditTrial
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NameOfEnglish { get; set; }
        public string? GroupName { get; set; }
        public PartnerType PartnerType { get; set; }
        public string? Region { get; set; }
        public string? Sales { get; set; }
        public string? RegionSalesDirector { get; set; }
        public string? Address { get; set; }
        public string? AddressOfEnglish { get; set; }
        public string? Contact { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Fax { get; set; }
        public string? Comments { get; set; }
        public System.Collections.Generic.List<DomainEvent> DomainEvents { get; set; } = new System.Collections.Generic.List<DomainEvent>();
    }
}
