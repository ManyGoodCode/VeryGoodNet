using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Delete
{

    public class DeleteVisitorCommandValidator : AbstractValidator<DeleteVisitorCommand>
    {
        public DeleteVisitorCommandValidator()
        {

            RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(0));
        }
    }
}
    

