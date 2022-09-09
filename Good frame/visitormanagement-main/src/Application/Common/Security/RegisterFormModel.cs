namespace CleanArchitecture.Blazor.Application.Common.Security
{
    /// <summary>
    /// 注册模型：  用户名 / 电子邮件 / 密码 / 确认密码 / 同意加入
    /// </summary>
    public class RegisterFormModel
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool AgreeToTerms { get; set; }
    }
}
