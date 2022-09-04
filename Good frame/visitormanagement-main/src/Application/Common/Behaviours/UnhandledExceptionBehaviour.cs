using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours
{

    public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<TRequest> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UnhandledExceptionBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                var userName = await _currentUserService.UserName();
                _logger.LogError(ex, "{Name}: {Exception} with {@Request} by {@UserName}", requestName, ex.Message, request, userName);
                throw;
            }
        }
    }
}