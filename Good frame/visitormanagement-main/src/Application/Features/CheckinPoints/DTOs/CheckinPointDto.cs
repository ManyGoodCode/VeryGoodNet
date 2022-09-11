// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Mappings;
using CleanArchitecture.Blazor.Application.Features.Devices.DTOs;
using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.DTOs
{
    public class CheckinPointDto : IMapFrom<CheckinPoint>
    {
        public void Mapping(Profile profile)
        {
            profile.CreateMap<CheckinPoint, CheckinPointDto>()
                   .ForMember(destinationMember: x => x.Devices, memberOptions: s => s.MapFrom(y => y.Devices.Select(x =>
                    new DeviceDto()
                    {
                       Name = x.Name,
                       Status = x.Status,
                       IPAddress = x.IPAddress
                    }).ToList()))
                   .ForMember(destinationMember: x => x.Site, memberOptions: s => s.MapFrom(y => y.Site.Name))
                   .ForMember(destinationMember: x => x.Address, memberOptions: s => s.MapFrom(y => y.Site.Address));
            profile.CreateMap<CheckinPointDto, CheckinPoint>()
                   .ForMember(destinationMember: x => x.Devices, memberOptions: opt => opt.Ignore())
                   .ForMember(destinationMember: x => x.Site, memberOptions: opt => opt.Ignore());
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IEnumerable<DeviceDto> Devices { get; set; } = new List<DeviceDto>();
        public int? SiteId { get; set; }
        public string? Site { get; set; }
        public string? Address { get; set; }
    }
}

