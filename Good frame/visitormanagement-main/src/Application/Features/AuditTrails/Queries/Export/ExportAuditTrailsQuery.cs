// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Extensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.AuditTrails.DTOs;
using CleanArchitecture.Blazor.Domain.Entities.Audit;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Application.Features.AuditTrails.Queries.Export
{
    public class ExportAuditTrailsQuery : IRequest<byte[]>
    {
        public string filterRules { get; set; }
        public string sort { get; set; } = "Id";
        public string order { get; set; } = "desc";
    }

    public class ExportAuditTrailsQueryHandler :
         IRequestHandler<ExportAuditTrailsQuery, byte[]>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IExcelService excelService;
        private readonly IStringLocalizer<ExportAuditTrailsQueryHandler> localizer;

        public ExportAuditTrailsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportAuditTrailsQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.excelService = excelService;
            this.localizer = localizer;
        }

        public async Task<byte[]> Handle(ExportAuditTrailsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<AuditTrail, bool>> filters = PredicateBuilder.FromFilter<AuditTrail>(request.filterRules);
            List<AuditTrailDto> data = await context.AuditTrails
                .Where(filters)
                //.OrderBy($"{request.sort} {request.order}")
                .ProjectTo<AuditTrailDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            byte[]? result = await excelService.ExportAsync(
                data: data,
                mappers: new Dictionary<string, Func<AuditTrailDto, object>>()
                {
                    { localizer["Date Time"], item => item.DateTime.ToString("yyyy-MM-dd HH:mm:ss") },
                    { localizer["Table Name"], item => item.TableName },
                    { localizer["Audit Type"], item => item.AuditType },
                    { localizer["Old Values"], item => item.OldValues },
                    { localizer["New Values"], item => item.NewValues },
                    { localizer["Primary Key"], item => item.PrimaryKey },
                },
               sheetName: localizer["AuditTrails"]);
            return result;
        }
    }
}

