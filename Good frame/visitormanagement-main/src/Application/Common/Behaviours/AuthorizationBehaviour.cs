using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Identity;
using CleanArchitecture.Blazor.Application.Common.Security;
using MediatR;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours
{

    public class AuthorizationBehaviour<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly IIdentityService identityService;

        public AuthorizationBehaviour(
            ICurrentUserService currentUserService,
            IIdentityService identityService)
        {
            this.currentUserService = currentUserService;
            this.identityService = identityService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            IEnumerable<RequestAuthorizeAttribute> authorizeAttributes = request.GetType().GetCustomAttributes<RequestAuthorizeAttribute>();
            if (authorizeAttributes.Any())
            {
                string userId = await currentUserService.UserId();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new UnauthorizedAccessException();
                }

                IEnumerable<RequestAuthorizeAttribute> authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));
                if (authorizeAttributesWithRoles.Any())
                {
                    bool authorized = false;
                    foreach (string[] roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                    {
                        foreach (string role in roles)
                        {
                            bool isInRole = await identityService.IsInRoleAsync(userId, role.Trim());
                            if (isInRole)
                            {
                                authorized = true;
                                break;
                            }
                        }
                    }

                    if (!authorized)
                    {
                        throw new ForbiddenAccessException("You are not authorized to access this resource.");
                    }
                }

                IEnumerable<RequestAuthorizeAttribute> authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
                if (authorizeAttributesWithPolicies.Any())
                {
                    foreach (string policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                    {
                        bool authorized = await identityService.AuthorizeAsync(userId, policy);
                        if (!authorized)
                        {
                            throw new ForbiddenAccessException("You are not authorized to access this resource.");
                        }
                    }
                }
            }

            return await next();
        }
    }

}