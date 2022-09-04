using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace CleanArchitecture.Blazor.Application.Common.Exceptions
{

    public class ValidationException : CustomException
    {
        public ValidationException(IEnumerable<ValidationFailure> failures) : base(string.Empty, failures
                 .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                 .Select(failureGroup => $"{string.Join(", ", failureGroup.Distinct().ToArray())}")
                 .ToList(), System.Net.HttpStatusCode.UnprocessableEntity)

        {

        }
    }
}
