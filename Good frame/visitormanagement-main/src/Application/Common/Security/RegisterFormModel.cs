namespace CleanArchitecture.Blazor.Application.Common.Security
{
    /// <summary>
    /// ע��ģ�ͣ�  �û��� / �����ʼ� / ���� / ȷ������ / ͬ�����
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
