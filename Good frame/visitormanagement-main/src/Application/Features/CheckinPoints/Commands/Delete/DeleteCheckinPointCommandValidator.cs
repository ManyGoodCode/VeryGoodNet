using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.CheckinPoints.Commands.Delete
{

    public class DeleteCheckinPointCommandValidator : AbstractValidator<DeleteCheckinPointCommand>
    {
        public DeleteCheckinPointCommandValidator()
        {
            RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(valueToCompare: 0));
        }
    }
}


