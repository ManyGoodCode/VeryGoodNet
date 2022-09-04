// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Services.MessageService;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.EventHandlers
{

    public class VisitorDeletedEventHandler : INotificationHandler<DomainEventNotification<DeletedEvent<Visitor>>>
    {
        private readonly ILogger<VisitorDeletedEventHandler> _logger;

        public VisitorDeletedEventHandler(
            IApplicationDbContext context,
        SMSMessageService sms,
        MailMessageService mail,
            ILogger<VisitorDeletedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(DomainEventNotification<DeletedEvent<Visitor>> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation("Domain Event Handle: {DomainEvent}", nameof(domainEvent));

            return Task.CompletedTask;
        }
    }
}
