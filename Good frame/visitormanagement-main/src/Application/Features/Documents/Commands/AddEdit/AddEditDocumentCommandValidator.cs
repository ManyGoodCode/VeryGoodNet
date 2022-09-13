using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Commands.AddEdit
{

    public class AddEditDocumentCommandValidator : AbstractValidator<AddEditDocumentCommand>
    {
        public AddEditDocumentCommandValidator()
        {
            RuleFor(v => v.Title)
                .NotNull()
                .MaximumLength(maximumLength:256)
                .NotEmpty();
            RuleFor(v => v.DocumentTypeId)
                .NotNull()
                .NotEqual(0);
            RuleFor(v => v.Description)
                .MaximumLength(maximumLength: 256);
            RuleFor(v => v.UploadRequest)
                .NotNull()
                .When(x => x.Id <= 0);
        }
    }
}
