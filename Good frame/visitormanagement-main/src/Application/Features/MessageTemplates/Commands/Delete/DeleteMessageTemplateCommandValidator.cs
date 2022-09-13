using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.MessageTemplates.Commands.Delete
{

    public class DeleteMessageTemplateCommandValidator : AbstractValidator<DeleteMessageTemplateCommand>
    {
        public DeleteMessageTemplateCommandValidator()
        {
            RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(0));
        }
    }
}


