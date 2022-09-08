using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{
    /// <summary>
    /// 当前用户服务。 获取用户Id / 获取用户名称 / 获取场所Id / 获取场所名称
    /// </summary>
    public interface ICurrentUserService
    {
        Task<string> UserId();
        Task<string> UserName();
        Task<int?> SiteId();
        Task<string> SiteName();
    }
}
