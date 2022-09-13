// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.DocumentTypes.DTOs;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.DocumentTypes.Queries.Export
{
    public class ExportDocumentTypesQuery : IRequest<byte[]>
    {
        public string Keyword { get; set; }
        public string OrderBy { get; set; } = "Id";
        public string SortDirection { get; set; } = "desc";
    }

    public class ExportDocumentTypesQueryHandler :
         IRequestHandler<ExportDocumentTypesQuery, byte[]>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IExcelService excelService;
        private readonly IStringLocalizer<ExportDocumentTypesQueryHandler> localizer;

        public ExportDocumentTypesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportDocumentTypesQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.excelService = excelService;
            this.localizer = localizer;
        }

        public async Task<byte[]> Handle(ExportDocumentTypesQuery request, CancellationToken cancellationToken)
        {
            List<DocumentTypeDto> data = await context.DocumentTypes.Where(x => x.Name.Contains(request.Keyword) || x.Description.Contains(request.Keyword))
                 //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                 .ProjectTo<DocumentTypeDto>(mapper.ConfigurationProvider)
                 .ToListAsync(cancellationToken);
            byte[] result = await excelService.ExportAsync(data,
                new Dictionary<string, Func<DocumentTypeDto, object>>()
                {
                    { localizer["Name"], item => item.Name },
                    { localizer["Description"], item => item.Description },

                }, localizer["DocumentTypes"]);
            return result;
        }
    }
}
