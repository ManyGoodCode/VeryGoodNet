using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentEmail.Core;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Services.MessageService
{
    /// <summary>
    /// 邮件发送服务
    /// </summary>
    public class MailMessageService
    {
        private readonly ILogger<MailMessageService> logger;
        private readonly IFluentEmail fluentEmail;

        public MailMessageService(ILogger<MailMessageService> logger,
            IFluentEmail fluentEmail)
        {
            this.logger = logger;
            this.fluentEmail = fluentEmail;
        }
        public async Task Send(string to, string subject, string body)
        {
            await fluentEmail.To(emailAddress: to)
                              .Subject(subject: subject)
                              .Body(body: body).SendAsync();
        }
    }
}
