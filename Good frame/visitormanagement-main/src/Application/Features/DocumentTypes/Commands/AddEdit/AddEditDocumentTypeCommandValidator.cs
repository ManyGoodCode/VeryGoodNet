using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.AddEdit
{

    public class AddEditDocumentTypeCommandValidator : AbstractValidator<AddEditDocumentTypeCommand>
    {
        public AddEditDocumentTypeCommandValidator()
        {
            RuleFor(v => v.Name)
                .MaximumLength(maximumLength: 256)
                .NotEmpty();
            RuleFor(v => v.Description)
                .MaximumLength(maximumLength: 512);
        }
    }
}
