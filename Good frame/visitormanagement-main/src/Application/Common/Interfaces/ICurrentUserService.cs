using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces
{

    public interface ICurrentUserService
    {
        Task<string> UserId();
        Task<string> UserName();
        Task<int?> SiteId();
        Task<string> SiteName();
    }
}
