// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.



using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Features.Documents.EventHandlers
{

    public class DocumentCreatedEventHandler : INotificationHandler<DomainEventNotification<DocumentCreatedEvent>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DocumentCreatedEventHandler> _logger;


        public DocumentCreatedEventHandler(
            IApplicationDbContext context,
            ILogger<DocumentCreatedEventHandler> logger

            )
        {
            _context = context;
            _logger = logger;

        }
        public async Task Handle(DomainEventNotification<DocumentCreatedEvent> notification, CancellationToken cancellationToken)
        {

            _logger.LogInformation($"Document Created");
        }
    }
}
