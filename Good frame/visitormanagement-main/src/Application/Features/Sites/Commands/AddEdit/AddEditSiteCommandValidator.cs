using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Sites.Commands.AddEdit
{
    public class AddEditSiteCommandValidator : AbstractValidator<AddEditSiteCommand>
    {
        public AddEditSiteCommandValidator()
        {
            RuleFor(v => v.Name)
              .MaximumLength(256)
               .NotEmpty();
            RuleFor(v => v.CompanyName)
                    .MaximumLength(256)
                     .NotEmpty();
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<AddEditSiteCommand>.CreateWithOptions((AddEditSiteCommand)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}

