// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.EventHandlers
{

    public class ApprovalHistoryCreatedEventHandler :
              INotificationHandler<DomainEventNotification<CreatedEvent<ApprovalHistory>>>
    {
        private readonly ILogger<ApprovalHistoryCreatedEventHandler> _logger;

        public ApprovalHistoryCreatedEventHandler(
            ILogger<ApprovalHistoryCreatedEventHandler> logger
            )
        {
            _logger = logger;
        }
        public Task Handle(DomainEventNotification<CreatedEvent<ApprovalHistory>> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation("{handler}: visitorId: {VisitorId}, {Outcome}",
                nameof(ApprovalHistoryCreatedEventHandler),
                domainEvent.Entity.VisitorId,
                $"{domainEvent.Entity.Outcome} {domainEvent.Entity.Comment}");

            return Task.CompletedTask;
        }
    }
}
