using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours
{
    /// <summary>
    /// 通过 Stopwatch 监控一个用户的请求耗时并记录日志
    /// </summary>
    public class PerformanceBehaviour<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch timer;
        private readonly ILogger<TRequest> logger;
        private readonly ICurrentUserService currentUserService;

        public PerformanceBehaviour(
            ILogger<TRequest> logger,
            ICurrentUserService currentUserService)
        {
            timer = new Stopwatch();

            this.logger = logger;
            this.currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            timer.Start();
            TResponse response = await next();
            timer.Stop();

            long elapsedMilliseconds = timer.ElapsedMilliseconds;
            if (elapsedMilliseconds > 500)
            {
                string requestName = typeof(TRequest).Name;
                string userName = await currentUserService.UserName();
                logger.LogWarning("{Name} long running request ({ElapsedMilliseconds} milliseconds) with {@Request} {@UserName} ",
                    requestName, elapsedMilliseconds, request, userName);
            }

            return response;
        }
    }
}