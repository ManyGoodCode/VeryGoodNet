namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs
{
    /// <summary>
    /// 请求令牌。用户名/密码/是否记住我
    /// </summary>
    public class TokenRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
