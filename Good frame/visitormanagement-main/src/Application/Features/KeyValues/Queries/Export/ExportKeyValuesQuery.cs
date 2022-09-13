using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.KeyValues.DTOs;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Application.Features.KeyValues.Queries.Export
{
    public class ExportKeyValuesQuery : IRequest<byte[]>
    {
        public string Keyword { get; set; }
        public string OrderBy { get; set; } = "Id";
        public string SortDirection { get; set; } = "desc";
    }

    public class ExportKeyValuesQueryHandler :
         IRequestHandler<ExportKeyValuesQuery, byte[]>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IExcelService excelService;
        private readonly IStringLocalizer<ExportKeyValuesQueryHandler> localizer;

        public ExportKeyValuesQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportKeyValuesQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.excelService = excelService;
            this.localizer = localizer;
        }

        public async Task<byte[]> Handle(ExportKeyValuesQuery request, CancellationToken cancellationToken)
        {
            List<KeyValueDto> data = await context.KeyValues.Where(x => x.Name.Contains(request.Keyword) || x.Value.Contains(request.Keyword) || x.Text.Contains(request.Keyword))
                //.OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectTo<KeyValueDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            byte[] result = await excelService.ExportAsync(
                data: data,
                mappers: new Dictionary<string, Func<KeyValueDto, object>>()
                {
                    //{ _localizer["Id"], item => item.Id },
                    { localizer["Name"], item => item.Name },
                    { localizer["Value"], item => item.Value },
                    { localizer["Text"], item => item.Text },
                    { localizer["Description"], item => item.Description },},
                sheetName: localizer["Data"]);
            return result;
        }
    }
}
