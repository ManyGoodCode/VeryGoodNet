namespace CleanArchitecture.Blazor.Application.Common.Security
{
    /// <summary>
    /// ��¼��ģ�͡��û��� / ���� / �Ƿ��ס��
    /// </summary>
    public class LoginFormModel
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}