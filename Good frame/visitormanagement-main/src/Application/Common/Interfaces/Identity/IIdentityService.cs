using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity
{
    /// <summary>
    /// 用户服务。通过令牌登录 / 刷新令牌请求 / 通过Id获取用户名 / 通过用户Id判断是否为某角色 / 用户Id授权 / 通过用户名和密码创建用户 / 通过用户Id删除用户 / 获取角色下的用户 / 更新用户在线状态
    /// </summary>
    public interface IIdentityService : IService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest request);
        Task<Result<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<string?> GetUserNameAsync(string userId);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task<bool> AuthorizeAsync(string userId, string policyName);
        Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);
        Task<Result> DeleteUserAsync(string userId);
        Task<IDictionary<string, string>> FetchUsers(string roleName);
        Task UpdateLiveStatus(string userId, bool isLive);
    }
}
