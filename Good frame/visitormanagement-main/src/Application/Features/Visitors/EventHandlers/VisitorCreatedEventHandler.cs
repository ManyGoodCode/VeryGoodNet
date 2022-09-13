using System;
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

    public class VisitorCreatedEventHandler : INotificationHandler<DomainEventNotification<CreatedEvent<Visitor>>>
    {
        private readonly IApplicationDbContext context;
        private readonly SMSMessageService sms;
        private readonly MailMessageService mail;
        private readonly ILogger<VisitorCreatedEventHandler> logger;

        public VisitorCreatedEventHandler(
            IApplicationDbContext context,
            SMSMessageService sms,
            MailMessageService mail,
            ILogger<VisitorCreatedEventHandler> logger)
        {
            this.context = context;
            this.sms = sms;
            this.mail = mail;
            this.logger = logger;
        }

        public async Task Handle(
            DomainEventNotification<CreatedEvent<Visitor>> notification,
            CancellationToken cancellationToken)
        {
            CreatedEvent<Visitor> domainEvent = notification.DomainEvent;
            Visitor visitor = domainEvent.Entity;
            if (visitor.PhoneNumber != null)
            {
                MessageTemplate template = await context.MessageTemplates.FirstOrDefaultAsync(x =>
                      x.SiteId == visitor.SiteId &&
                      x.MessageType == MessageType.Sms &&
                      x.ForStatus == visitor.Status, cancellationToken);

                if (template != null)
                {
                    await sms.Send(visitor.PhoneNumber, new string[] { String.Format(template.Body, visitor.PassCode) }, template.Subject);
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
                    await mail.Send(visitor.Email, template.Subject, string.Format(template.Body, visitor.PassCode));
                }
            }
        }
    }
}
