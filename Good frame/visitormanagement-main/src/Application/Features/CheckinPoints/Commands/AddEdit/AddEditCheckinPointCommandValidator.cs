using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.Commands.AddEdit
{

    public class AddEditCheckinPointCommandValidator : AbstractValidator<AddEditCheckinPointCommand>
    {
        public AddEditCheckinPointCommandValidator()
        {
            RuleFor(v => v.Name)
                .MaximumLength(256)
                .NotEmpty();
            RuleFor(v => v.SiteId).NotNull().GreaterThan(0);
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<AddEditCheckinPointCommand>.CreateWithOptions((AddEditCheckinPointCommand)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}

