using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using LazyCache;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours
{

    public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICacheable
    {
        private readonly IAppCache _cache;
        private readonly ILogger<CachingBehaviour<TRequest, TResponse>> _logger;

        public CachingBehaviour(
            IAppCache cache,
            ILogger<CachingBehaviour<TRequest, TResponse>> logger
            )
        {
            _cache = cache;
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!string.IsNullOrEmpty(request.CacheKey))
            {
                _logger.LogTrace("{Name} is caching with {@Request}.", nameof(request), request);
                var response = await _cache.GetOrAddAsync(
                    request.CacheKey,
                    async () => await next(),
                    request.Options);
                return response;
            }
            else
            {
                return await next();
            }
        }
    }
}
