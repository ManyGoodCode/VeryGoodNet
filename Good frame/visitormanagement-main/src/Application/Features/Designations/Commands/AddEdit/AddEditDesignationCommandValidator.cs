// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Designations.Commands.AddEdit
{

    public class AddEditDesignationCommandValidator : AbstractValidator<AddEditDesignationCommand>
    {
        public AddEditDesignationCommandValidator()
        {
            RuleFor(v => v.Name)
                   .MaximumLength(256)
                   .NotEmpty();
            RuleFor(v => v.Status)
                     .MaximumLength(256)
                     .NotEmpty();
        }
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<AddEditDesignationCommand>.CreateWithOptions((AddEditDesignationCommand)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}

