using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Settings;
using CleanArchitecture.Blazor.Infrastructure.Configurations;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CleanArchitecture.Blazor.Infrastructure.Services
{
    public class SMTPMailService : IMailService
    {
        public MailSettings mailSettings { get; }
        public ILogger<SMTPMailService> logger { get; }

        public SMTPMailService(IOptions<MailSettings> mailSettings, ILogger<SMTPMailService> logger)
        {
            this.mailSettings = mailSettings.Value;
            this.logger = logger;
        }

        public async Task SendAsync(MailRequest request)
        {
            try
            {
                MimeMessage email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(request.From ?? mailSettings.From);
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                BodyBuilder builder = new BodyBuilder();
                builder.HtmlBody = request.Body;
                email.Body = builder.ToMessageBody();
                using SmtpClient smtp = new SmtpClient();
                smtp.Connect(mailSettings.Host, mailSettings.Port, true);
                smtp.Authenticate(mailSettings.UserName, mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex.Message, ex);
            }
        }
    }
}
