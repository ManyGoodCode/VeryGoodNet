namespace CleanArchitecture.Blazor.Application.Common.Security
{
    /// <summary>
    /// 登录表单模型。用户名 / 密码 / 是否记住我
    /// </summary>
    public class LoginFormModel
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}