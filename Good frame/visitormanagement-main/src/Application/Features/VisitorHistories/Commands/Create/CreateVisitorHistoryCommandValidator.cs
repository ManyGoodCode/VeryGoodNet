using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Features.VisitorHistories.Constants;
using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.VisitorHistories.Commands.Create
{

    public class CreateVisitorHistoryCommandValidator : AbstractValidator<CreateVisitorHistoryCommand>
    {
        public CreateVisitorHistoryCommandValidator()
        {
            RuleFor(v => v.VisitorId).NotNull();
            RuleFor(v => v.Visitor).NotEmpty().NotNull();
            RuleFor(v => v.CheckinPointId).NotNull();
            RuleFor(v => v.Temperature).NotNull().When(x => x.Stage == CheckStage.Checkin);
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
       {
           FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<CreateVisitorHistoryCommand>.CreateWithOptions((CreateVisitorHistoryCommand)model, x => x.IncludeProperties(propertyName)));
           if (result.IsValid)
               return Array.Empty<string>();
           return result.Errors.Select(e => e.ErrorMessage);
       };
    }
}

