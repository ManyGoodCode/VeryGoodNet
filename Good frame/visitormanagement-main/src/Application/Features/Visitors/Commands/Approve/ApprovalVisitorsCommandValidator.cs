using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Approve
{

    public class ApprovalVisitorsCommandValidator : AbstractValidator<ApprovalVisitorsCommand>
    {
        public ApprovalVisitorsCommandValidator()
        {
            RuleFor(x => x.VisitorId).NotEmpty();
            RuleFor(x => x.Outcome).NotEmpty();
        }
    }
}

