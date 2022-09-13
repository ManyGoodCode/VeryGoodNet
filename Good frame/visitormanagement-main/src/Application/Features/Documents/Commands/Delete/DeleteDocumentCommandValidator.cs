using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Documents.Commands.Delete
{

    public class DeleteDocumentCommandValidator : AbstractValidator<DeleteDocumentCommand>
    {
        public DeleteDocumentCommandValidator()
        {
            RuleFor(expression: x => x.Id).NotNull().NotEmpty();
        }
    }
}

