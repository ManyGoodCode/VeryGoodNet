using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace CleanArchitecture.Blazor.Application.Common.Exceptions
{
    /// <summary>
    /// 验证器异常【分组排序】
    /// </summary>
    public class ValidationException : CustomException
    {
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : base(string.Empty, failures
                 .GroupBy(keySelector: e => e.PropertyName, elementSelector: e => e.ErrorMessage)
                 .Select(selector: failureGroup => $"{string.Join(", ", failureGroup.Distinct().ToArray())}")
                 .ToList(), System.Net.HttpStatusCode.UnprocessableEntity)

        {

        }
    }
}
