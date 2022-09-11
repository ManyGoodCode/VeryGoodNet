using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.EventHandlers
{
    /// <summary>
    /// 审批请求处理器
    /// 目的：日志记录审批对象
    /// </summary>
    public class ApprovalHistoryCreatedEventHandler :
              INotificationHandler<DomainEventNotification<CreatedEvent<ApprovalHistory>>>
    {
        private readonly ILogger<ApprovalHistoryCreatedEventHandler> logger;

        public ApprovalHistoryCreatedEventHandler(
            ILogger<ApprovalHistoryCreatedEventHandler> logger)
        {
            this.logger = logger;
        }
        public Task Handle(
            DomainEventNotification<CreatedEvent<ApprovalHistory>> notification,
            CancellationToken cancellationToken)
        {
            CreatedEvent<ApprovalHistory> domainEvent = notification.DomainEvent;
            logger.LogInformation("{handler}: visitorId: {VisitorId}, {Outcome}",
                nameof(ApprovalHistoryCreatedEventHandler),
                domainEvent.Entity.VisitorId,
                $"{domainEvent.Entity.Outcome} {domainEvent.Entity.Comment}");

            return Task.CompletedTask;
        }
    }
}
