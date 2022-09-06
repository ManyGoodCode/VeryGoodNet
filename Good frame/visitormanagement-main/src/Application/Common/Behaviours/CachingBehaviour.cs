using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using LazyCache;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours
{
    /// <summary>
    /// 往缓存中添加TRequest的键值
    /// </summary>
    public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICacheable
    {
        private readonly IAppCache cache;
        private readonly ILogger<CachingBehaviour<TRequest, TResponse>> logger;

        public CachingBehaviour(
            IAppCache cache,
            ILogger<CachingBehaviour<TRequest, TResponse>> logger
            )
        {
            this.cache = cache;
            this.logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!string.IsNullOrEmpty(request.CacheKey))
            {
                logger.LogTrace("{Name} is caching with {@Request}.", nameof(request), request);
                TResponse response = await cache.GetOrAddAsync(
                    key: request.CacheKey,
                    addItemFactory: async () => await next(),
                    policy: request.Options);

                return response;
            }
            else
            {
                return await next();
            }
        }
    }
}
