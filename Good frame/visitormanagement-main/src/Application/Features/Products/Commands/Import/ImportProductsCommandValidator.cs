using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Products.Commands.Import
{

    public class ImportProductsCommandValidator : AbstractValidator<ImportProductsCommand>
    {
        public ImportProductsCommandValidator()
        {
            RuleFor(v => v.Data)
                  .NotNull()
                 .NotEmpty();
        }
    }
}
