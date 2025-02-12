// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using FluentValidation;

namespace CleanArchitecture.Blazor.Application.Features.Employees.Commands.Delete
{

    public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
    {
        public DeleteEmployeeCommandValidator()
        {
            RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(0));
        }
    }
}


