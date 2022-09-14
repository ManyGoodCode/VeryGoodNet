using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.Sites.DTOs
{

    public class SiteDto : IMapFrom<Site>
    {
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Site, SiteDto>()
                   .ForMember(x => x.CheckinPoints, s => s.MapFrom(y => y.CheckinPoints.Select(x => x.Name).ToArray()));
            profile.CreateMap<SiteDto, Site>().ForMember(x => x.CheckinPoints, opt => opt.Ignore());
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public IEnumerable<string?> CheckinPoints { get; set; } = new List<string>();
    }
}

