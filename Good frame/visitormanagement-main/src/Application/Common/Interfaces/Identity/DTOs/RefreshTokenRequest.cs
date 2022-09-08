namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs
{
    /// <summary>
    /// 刷新令牌请求
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// 令牌
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; }
    }
}
