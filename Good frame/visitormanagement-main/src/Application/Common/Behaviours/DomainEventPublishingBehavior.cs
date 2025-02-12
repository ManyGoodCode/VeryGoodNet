using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Domain.Common;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours
{
    /// <summary>
    /// 查询数据库里面的 Domain 事件实体 属性 IsPublished 为False的，通过 IDomainEventService 发布出去并设置IsPublished
    /// </summary>
    public class DomainEventPublishingBehaviour<TRequest, TResponse> :
        IRequestPostProcessor<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger logger;
        private readonly IApplicationDbContext dbContext;
        private readonly IDomainEventService domainEventService;

        public DomainEventPublishingBehaviour(
            ILogger<TRequest> logger,
            IApplicationDbContext dbContext,
            IDomainEventService domainEventService)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.domainEventService = domainEventService;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            while (true)
            {
                DomainEvent? domainEventEntity = dbContext.ChangeTracker.Entries<IHasDomainEvent>()
                                                     .Select(x => x.Entity.DomainEvents)
                                                     .SelectMany(x => x)
                                                     .Where(domainEvent => !domainEvent.IsPublished)
                                                     .FirstOrDefault();
                if (domainEventEntity == null)
                    break;

                domainEventEntity.IsPublished = true;
                await domainEventService.Publish(domainEventEntity);
                logger.LogTrace("Published event: {Name}", nameof(domainEventEntity));
            }
        }
    }
}