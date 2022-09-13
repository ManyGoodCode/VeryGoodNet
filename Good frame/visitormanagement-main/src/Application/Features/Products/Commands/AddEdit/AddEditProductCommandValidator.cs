using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.AddEdit
{
    public class AddEditProductCommandValidator : AbstractValidator<AddEditProductCommand>
    {
        public AddEditProductCommandValidator()
        {
            RuleFor(v => v.Name)
                  .MaximumLength(256)
                  .NotEmpty();
            RuleFor(v => v.Unit)
                .MaximumLength(2)
                .NotEmpty();
            RuleFor(v => v.Brand)
               .MaximumLength(30)
               .NotEmpty();
            RuleFor(v => v.Price)
                   .GreaterThanOrEqualTo(0);
            RuleFor(v => v.Description)
                       .MaximumLength(1024);
            RuleFor(v => v.Pictures)
                .NotEmpty();
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<AddEditProductCommand>.CreateWithOptions((AddEditProductCommand)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}

