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
        private readonly IAppCache _cache;
        private readonly ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> _logger;

        public CacheInvalidationBehaviour(
            IAppCache cache,
            ILogger<CacheInvalidationBehaviour<TRequest, TResponse>> logger
            )
        {
            _cache = cache;
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogTrace("{Name} cache expire with {@Request}.", nameof(request), request);
            var response = await next();
            if (!string.IsNullOrEmpty(request.CacheKey))
            {
                _cache.Remove(request.CacheKey);
            }

            //request.SharedExpiryTokenSource?.TryReset();
            return response;
        }
    }
}
