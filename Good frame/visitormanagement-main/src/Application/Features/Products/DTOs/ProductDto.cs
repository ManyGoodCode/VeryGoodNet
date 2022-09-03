using System.Collections.Generic;

namespace CleanArchitecture.Blazor.Application.Features.Products.DTOs
{


    public class ProductDto : IMapFrom<Product>
    {

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public string? Brand { get; set; }
        public decimal Price { get; set; }
        public IList<string>? Pictures { get; set; }
    }
}

