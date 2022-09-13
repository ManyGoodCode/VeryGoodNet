using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.Import
{

    public class ImportDocumentTypesCommandValidator : AbstractValidator<ImportDocumentTypesCommand>
    {
        public ImportDocumentTypesCommandValidator()
        {
            RuleFor(x => x.Data).NotNull().NotEmpty();
        }
    }
}