using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using ValidationException = CleanArchitecture.Blazor.Application.Common.Exceptions.ValidationException;

namespace CleanArchitecture.Blazor.Application.Common.Behaviours
{
    /// <summary>
    /// 验证一个请求是否满足否则抛出异常，通过一系列验证器集合
    /// </summary>
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (validators.Any())
            {
                foreach (IValidator<TRequest> validator in validators)
                    validator.ValidateAndThrow(request);
            }

            return await next();
        }
    }
}
