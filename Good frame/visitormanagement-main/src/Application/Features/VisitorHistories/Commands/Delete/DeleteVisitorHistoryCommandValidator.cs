using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.VisitorHistories.Commands.Delete
{

    public class DeleteVisitorHistoryCommandValidator : AbstractValidator<DeleteVisitorHistoryCommand>
    {
        public DeleteVisitorHistoryCommandValidator()
        {
            RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(0));
        }
    }
}
    

