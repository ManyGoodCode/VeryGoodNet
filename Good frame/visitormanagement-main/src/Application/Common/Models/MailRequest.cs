
namespace CleanArchitecture.Blazor.Application.Settings
{
    /// <summary>
    /// 邮件请求。 发件人 / 收件人 / 主题 / 内容
    /// </summary>
    public class MailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
    }
}
