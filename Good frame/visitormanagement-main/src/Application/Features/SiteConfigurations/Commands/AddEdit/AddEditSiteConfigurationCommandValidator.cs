using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Commands.AddEdit
{
    public class AddEditSiteConfigurationCommandValidator : AbstractValidator<AddEditSiteConfigurationCommand>
    {
        public AddEditSiteConfigurationCommandValidator()
        {
            RuleFor(v => v.SiteId)
                         .NotEmpty();
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<AddEditSiteConfigurationCommand>.CreateWithOptions((AddEditSiteConfigurationCommand)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}

