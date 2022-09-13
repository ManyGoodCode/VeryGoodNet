using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.MessageTemplates.Commands.AddEdit
{

    public class AddEditMessageTemplateCommandValidator : AbstractValidator<AddEditMessageTemplateCommand>
    {
        public AddEditMessageTemplateCommandValidator()
        {
            RuleFor(v => v.ForStatus)
                   .MaximumLength(256)
                   .NotEmpty();
            RuleFor(v => v.MessageType).IsInEnum();
            RuleFor(v => v.SiteId)
                     .NotNull();
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<AddEditMessageTemplateCommand>.CreateWithOptions((AddEditMessageTemplateCommand)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}

