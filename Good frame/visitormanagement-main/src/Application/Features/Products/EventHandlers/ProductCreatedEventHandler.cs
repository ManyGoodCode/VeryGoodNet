using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Products.EventHandlers
{

    public class ProductCreatedEventHandler : INotificationHandler<DomainEventNotification<CreatedEvent<Product>>>
    {
        private readonly ILogger<ProductCreatedEventHandler> logger;

        public ProductCreatedEventHandler(
            ILogger<ProductCreatedEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(DomainEventNotification<CreatedEvent<Product>> notification, CancellationToken cancellationToken)
        {
            CreatedEvent<Product> domainEvent = notification.DomainEvent;
            logger.LogInformation("Domain Event: {DomainEvent}", domainEvent.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
