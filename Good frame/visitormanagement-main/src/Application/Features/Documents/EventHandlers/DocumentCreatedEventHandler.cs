using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers
{

    public class DocumentCreatedEventHandler : INotificationHandler<DomainEventNotification<DocumentCreatedEvent>>
    {
        private readonly IApplicationDbContext context;
        private readonly ILogger<DocumentCreatedEventHandler> logger;


        public DocumentCreatedEventHandler(
            IApplicationDbContext context,
            ILogger<DocumentCreatedEventHandler> logger)
        {
            this.context = context;
            this.logger = logger;

        }
        public async Task Handle(
            DomainEventNotification<DocumentCreatedEvent> notification,
            CancellationToken cancellationToken)
        {
            logger.LogInformation($"Document Created");
        }
    }
}
