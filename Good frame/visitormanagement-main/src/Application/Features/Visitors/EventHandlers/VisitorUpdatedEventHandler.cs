using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.Visitors.Constant;
using CleanArchitecture.Blazor.Application.Services.MessageService;
using CleanArchitecture.Blazor.Domain;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.EventHandlers
{

    public class VisitorUpdatedEventHandler : INotificationHandler<DomainEventNotification<UpdatedEvent<Visitor>>>
    {
        private readonly IApplicationDbContext context;
        private readonly SMSMessageService sms;
        private readonly MailMessageService mail;
        private readonly ILogger<VisitorUpdatedEventHandler> logger;

        public VisitorUpdatedEventHandler(
            IApplicationDbContext context,
            SMSMessageService sms,
            MailMessageService mail,
            ILogger<VisitorUpdatedEventHandler> logger)
        {
            this.context = context;
            this.sms = sms;
            this.mail = mail;
            this.logger = logger;
        }

        public async Task Handle(DomainEventNotification<UpdatedEvent<Visitor>> notification, CancellationToken cancellationToken)
        {
            UpdatedEvent<Visitor> domainEvent = notification.DomainEvent;
            Visitor visitor = domainEvent.Entity;
            if (visitor.PhoneNumber != null)
            {
                MessageTemplate template = await context.MessageTemplates.FirstOrDefaultAsync(x =>
                      x.SiteId == visitor.SiteId &&
                      x.MessageType == MessageType.Sms &&
                      x.ForStatus == visitor.Status, cancellationToken);
                if (template != null)
                {
                    if (visitor.Status == VisitorStatus.PendingApproval || visitor.Status == VisitorStatus.PendingConfirm)
                    {
                        Employee emp = context.Employees.FirstOrDefault(x => x.Id == visitor.EmployeeId);
                        if (emp != null)
                        {
                            await sms.Send(emp.PhoneNumber, new string[] { string.Format(template.Body, visitor.PassCode) }, template.Subject);
                        }
                    }
                    else
                    {
                        await sms.Send(visitor.PhoneNumber, new string[] { string.Format(template.Body, visitor.PassCode) }, template.Subject);
                    }

                }
            }
            if (visitor.Email != null)
            {
                MessageTemplate template = await context.MessageTemplates.FirstOrDefaultAsync(x =>
                      x.SiteId == visitor.SiteId &&
                      x.MessageType == MessageType.Email &&
                      x.ForStatus == visitor.Status, cancellationToken);
                if (template != null)
                {
                    if (visitor.Status == VisitorStatus.PendingApproval || visitor.Status == VisitorStatus.PendingConfirm)
                    {
                        Employee emp = context.Employees.FirstOrDefault(x => x.Id == visitor.EmployeeId);
                        if (emp != null)
                        {
                            await mail.Send(emp.Email, template.Subject, string.Format(template.Body, visitor.PassCode));
                        }
                    }
                    else
                    {
                        await mail.Send(visitor.Email, template.Subject, string.Format(template.Body, visitor.PassCode));
                    }
                }
            }
        }
    }
}
