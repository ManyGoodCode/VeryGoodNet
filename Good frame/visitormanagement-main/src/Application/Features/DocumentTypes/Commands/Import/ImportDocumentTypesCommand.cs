// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Interfaces.Caching;
using CleanArchitecture.Blazor.Application.Common.Models;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.Caching;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.AddEdit;
using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Commands.Import
{

    public class ImportDocumentTypesCommand : IRequest<Result>, ICacheInvalidator
    {
        public string FileName { get; set; } = default!;
        public byte[] Data { get; set; } = default!;
        public CancellationTokenSource? SharedExpiryTokenSource => DocumentTypeCacheKey.SharedExpiryTokenSource;
        public ImportDocumentTypesCommand(string fileName, byte[] data)
        {
            FileName = fileName;
            Data = data;
        }
    }

    public class CreateDocumentTypeTemplateCommand : IRequest<byte[]>
    {

    }
    public class ImportDocumentTypesCommandHandler :
        IRequestHandler<CreateDocumentTypeTemplateCommand, byte[]>,
        IRequestHandler<ImportDocumentTypesCommand, Result>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IExcelService excelService;
        private readonly IStringLocalizer<ImportDocumentTypesCommandHandler> localizer;
        private readonly IValidator<AddEditDocumentTypeCommand> addValidator;

        public ImportDocumentTypesCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ImportDocumentTypesCommandHandler> localizer,
            IValidator<AddEditDocumentTypeCommand> addValidator
            )
        {
            this.context = context;
            this.mapper = mapper;
            this.excelService = excelService;
            this.localizer = localizer;
            this.addValidator = addValidator;
        }
        public async Task<Result> Handle(ImportDocumentTypesCommand request, CancellationToken cancellationToken)
        {
            IResult<IEnumerable<DocumentType>> result = await excelService.ImportAsync(request.Data, mappers: new Dictionary<string, Func<DataRow, DocumentType, object>>
            {
                { localizer["Name"], (row,item) => item.Name = row[localizer["Name"]]?.ToString() },
                { localizer["Description"], (row,item) => item.Description =  row[localizer["Description"]]?.ToString() }
            }, localizer["DocumentTypes"]);

            if (result.Succeeded)
            {
                IEnumerable<DocumentType> importItems = result.Data;
                List<string> errors = new List<string>();
                bool errorsOccurred = false;
                foreach (DocumentType item in importItems)
                {
                    FluentValidation.Results.ValidationResult validationResult = await addValidator.ValidateAsync(mapper.Map<AddEditDocumentTypeCommand>(item), cancellationToken);
                    if (validationResult.IsValid)
                    {
                        bool exist = await context.DocumentTypes.AnyAsync(x => x.Name == item.Name, cancellationToken);
                        if (!exist)
                        {
                            await context.DocumentTypes.AddAsync(item, cancellationToken);
                        }
                    }
                    else
                    {
                        errorsOccurred = true;
                        errors.AddRange(validationResult.Errors.Select(e => $"{(!string.IsNullOrWhiteSpace(item.Name) ? $"{item.Name} - " : string.Empty)}{e.ErrorMessage}"));
                    }
                }

                if (errorsOccurred)
                {
                    return await Result.FailureAsync(errors);
                }

                await context.SaveChangesAsync(cancellationToken);
                return await Result.SuccessAsync();
            }
            else
            {
                return await Result.FailureAsync(result.Errors);
            }
        }

        public async Task<byte[]> Handle(CreateDocumentTypeTemplateCommand request, CancellationToken cancellationToken)
        {
            string[] fields = new string[] { localizer["Name"], localizer["Description"] };
            byte[] result = await excelService.CreateTemplateAsync(fields, localizer["DocumentTypes"]);
            return result;
        }
    }
}
