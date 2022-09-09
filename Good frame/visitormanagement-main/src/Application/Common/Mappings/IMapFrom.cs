using AutoMapper;

namespace CleanArchitecture.Blazor.Application.Common.Mappings
{

    public interface IMapFrom<T>
    {
        /// <summary>
        /// 定义映射接口
        /// </summary>
        void Mapping(AutoMapper.Profile profile) =>
            profile.CreateMap(sourceType: typeof(T), destinationType: GetType(), memberList: MemberList.None).ReverseMap();
    }
}
