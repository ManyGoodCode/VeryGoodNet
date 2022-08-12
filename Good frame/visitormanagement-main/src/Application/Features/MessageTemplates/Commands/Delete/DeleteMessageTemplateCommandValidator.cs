// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.MessageTemplates.Commands.Delete;

public class DeleteMessageTemplateCommandValidator : AbstractValidator<DeleteMessageTemplateCommand>
{
    public DeleteMessageTemplateCommandValidator()
    {

        RuleFor(v => v.Id).NotNull().ForEach(v => v.GreaterThan(0));

    }
}


