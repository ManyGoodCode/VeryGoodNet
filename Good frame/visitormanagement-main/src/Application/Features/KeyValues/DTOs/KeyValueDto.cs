using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Domain;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs
{

    public partial class KeyValueDto : IMapFrom<KeyValue>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Value { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }
        public TrackingState TrackingState { get; set; } = TrackingState.Unchanged;

    }
}
