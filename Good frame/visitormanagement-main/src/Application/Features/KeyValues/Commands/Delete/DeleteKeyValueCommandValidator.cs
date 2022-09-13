using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Commands.Delete
{

    public class DeleteKeyValueCommandValidator : AbstractValidator<DeleteKeyValueCommand>
    {
        public DeleteKeyValueCommandValidator()
        {
            RuleFor(expression: x => x.Id).NotNull().NotEmpty();
        }
    }
}
