using System;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs
{
    /// <summary>
    /// 返回令牌。令牌/刷新令牌/刷新令牌的过期时间
    /// </summary>
    public class TokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string ProfilePictureDataUrl { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
