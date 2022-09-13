using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Checking
{

    public class CheckingVisitorsCommandValidator : AbstractValidator<CheckingVisitorsCommand>
    {
        public CheckingVisitorsCommandValidator()
        {
            RuleFor(x => x.VisitorId).NotEmpty();
            RuleFor(x => x.Outcome).NotEmpty();
        }
    }
}

