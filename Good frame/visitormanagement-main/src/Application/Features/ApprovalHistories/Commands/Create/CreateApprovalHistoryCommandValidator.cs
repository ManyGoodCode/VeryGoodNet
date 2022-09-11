// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.ApprovalHistories.Commands.Create
{
    /// <summary>
    /// 审批请求验证器
    /// </summary>
    public class CreateApprovalHistoryCommandValidator : AbstractValidator<CreateApprovalHistoryCommand>
    {
        public CreateApprovalHistoryCommandValidator()
        {
            RuleFor(v => v.Outcome)
                 .MaximumLength(maximumLength: 256)
                 .NotEmpty();
            RuleFor(v => v.VisitorId).NotNull().GreaterThan(valueToCompare: 0);
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<CreateApprovalHistoryCommand>.CreateWithOptions((CreateApprovalHistoryCommand)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}

