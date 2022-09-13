using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.SiteConfigurations.Commands.Delete
{

    public class DeleteSiteConfigurationCommandValidator : AbstractValidator<DeleteSiteConfigurationCommand>
    {
        public DeleteSiteConfigurationCommandValidator()
        {
            RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(0));
        }
    }
}
    

