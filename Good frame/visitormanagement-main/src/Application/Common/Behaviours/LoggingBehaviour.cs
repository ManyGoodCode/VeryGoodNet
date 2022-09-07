using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours
{
    /// <summary>
    /// 记录请求的用户日志
    /// </summary>
    public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
    {
        private readonly ILogger logger;
        private readonly ICurrentUserService currentUserService;


        public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            this.logger = logger;
            this.currentUserService = currentUserService;

        }

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            string requestName = nameof(TRequest);
            string userName = await currentUserService.UserName();
            logger.LogTrace("Request: {Name} with {@Request} by {@UserName}",
                requestName, request, userName);
        }
    }
}
