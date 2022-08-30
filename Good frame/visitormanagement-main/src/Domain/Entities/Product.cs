using System.Collections.Generic;
using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Entities
{
    public class Product : AuditableEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public string? Unit { get; set; }
        public decimal Price { get; set; }
        public IList<string>? Pictures { get; set; }
    }
}
