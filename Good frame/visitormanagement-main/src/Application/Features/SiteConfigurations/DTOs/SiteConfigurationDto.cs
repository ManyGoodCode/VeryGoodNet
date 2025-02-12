using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.SiteConfigurations.DTOs
{
    public class SiteConfigurationDto : IMapFrom<SiteConfiguration>
    {
        public void Mapping(Profile profile)
        {
            profile.CreateMap<SiteConfiguration, SiteConfigurationDto>(MemberList.None)
                   .ForMember(x => x.SiteName, s => s.MapFrom(y => y.Site.Name))
                   .ForMember(x => x.CompanyName, s => s.MapFrom(y => y.Site.CompanyName));
            profile.CreateMap<SiteConfigurationDto, SiteConfiguration>(MemberList.None);
        }

        public int Id { get; set; }
        public int? SiteId { get; set; }
        public string? SiteName { get; set; }
        public string? CompanyName { get; set; }
        public bool MandatoryHealthQrCode { get; set; } = true;
        public bool MandatoryTripCode { get; set; } = true;
        public bool MandatoryNucleicAcidTestReport { get; set; } = false;
        public string? Parameters { get; set; }
        public string? Description { get; set; }
    }
}

