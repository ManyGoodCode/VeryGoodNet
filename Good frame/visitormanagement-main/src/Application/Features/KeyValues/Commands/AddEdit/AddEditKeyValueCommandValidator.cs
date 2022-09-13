using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.AddEdit
{

    public class AddEditKeyValueCommandValidator : AbstractValidator<AddEditKeyValueCommand>
    {
        public AddEditKeyValueCommandValidator()
        {
            RuleFor(v => v.Name)
                .MaximumLength(256)
                .NotEmpty();
            RuleFor(v => v.Text)
                .MaximumLength(256)
                .NotEmpty();
            RuleFor(v => v.Value)
                .MaximumLength(256)
                .NotEmpty();
        }
    }
}
