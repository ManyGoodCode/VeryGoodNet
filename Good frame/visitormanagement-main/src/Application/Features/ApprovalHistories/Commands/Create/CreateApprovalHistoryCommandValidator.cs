// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Commands.Create
{

    public class CreateApprovalHistoryCommandValidator : AbstractValidator<CreateApprovalHistoryCommand>
    {
        public CreateApprovalHistoryCommandValidator()
        {

            RuleFor(v => v.Outcome)
                 .MaximumLength(256)
                 .NotEmpty();
            RuleFor(v => v.VisitorId).NotNull().GreaterThan(0);
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
     {
         var result = await ValidateAsync(ValidationContext<CreateApprovalHistoryCommand>.CreateWithOptions((CreateApprovalHistoryCommand)model, x => x.IncludeProperties(propertyName)));
         if (result.IsValid)
             return Array.Empty<string>();
         return result.Errors.Select(e => e.ErrorMessage);
     };
    }
}

