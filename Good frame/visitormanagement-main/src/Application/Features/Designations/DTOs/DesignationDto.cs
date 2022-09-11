using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Designations.DTOs
{
    public class DesignationDto : IMapFrom<Designation>
    {
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Designation, DesignationDto>().ReverseMap();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
    }
}

