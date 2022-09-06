using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using LazyCache;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours
{
    public class CacheInvalidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
          where TRequest : IRequest<TResponse>, ICacheInvalidator
    {
        private readonly IAppCache cache;
        private readonly ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger;

        public CacheInvalidationBehaviour(
            IAppCache cache,
            ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger
            )
        {
            this.cache = cache;
            this.logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            logger.LogTrace("{Name} cache expire with {@Request}.", nameof(request), request);
            TResponse response = await next();
            if (!string.IsNullOrEmpty(request.CacheKey))
            {
                cache.Remove(request.CacheKey);
            }

            return response;
        }
    }
}
