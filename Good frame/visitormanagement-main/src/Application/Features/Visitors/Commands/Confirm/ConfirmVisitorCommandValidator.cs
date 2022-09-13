using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Visitors.Commands.Approve
{

    public class ConfirmVisitorCommandValidator : AbstractValidator<ConfirmVisitorCommand>
    {
        public ConfirmVisitorCommandValidator()
        {
            RuleFor(x => x.VisitorId).NotEmpty();
        }
    }
}

