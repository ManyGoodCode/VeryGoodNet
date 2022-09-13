using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Sites.Commands.Delete
{

    public class DeleteSiteCommandValidator : AbstractValidator<DeleteSiteCommand>
    {
        public DeleteSiteCommandValidator()
        {

            RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(0));
        }
    }
}
    

