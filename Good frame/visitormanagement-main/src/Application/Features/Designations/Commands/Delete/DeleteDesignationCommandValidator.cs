using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Designations.Commands.Delete
{

    public class DeleteDesignationCommandValidator : AbstractValidator<DeleteDesignationCommand>
    {
        public DeleteDesignationCommandValidator()
        {
            RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(0));
        }
    }
}


