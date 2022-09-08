using AutoMapper;

namespace CleanArchitecture.Blazor.Application.Common.Mappings
{

    public interface IMapFrom<T>
    {
        void Mapping(AutoMapper.Profile profile) => profile.CreateMap(sourceType: typeof(T), destinationType: GetType(), memberList: MemberList.None).ReverseMap();
    }
}
