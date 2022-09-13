using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.Delete
{

    public class DeleteDocumentTypeCommandValidator : AbstractValidator<DeleteDocumentTypeCommand>
    {
        public DeleteDocumentTypeCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
        }
    }
}
