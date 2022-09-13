// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers
{

    public class ProductDeletedEventHandler : INotificationHandler<DomainEventNotification<DeletedEvent<Product>>>
    {
        private readonly ILogger<ProductDeletedEventHandler> logger;

        public ProductDeletedEventHandler(
            ILogger<ProductDeletedEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(DomainEventNotification<DeletedEvent<Product>> notification, CancellationToken cancellationToken)
        {
            DeletedEvent<Product> domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
