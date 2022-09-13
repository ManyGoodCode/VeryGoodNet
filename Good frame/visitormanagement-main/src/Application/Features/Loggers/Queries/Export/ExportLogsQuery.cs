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
using CleanArchitecture.Blazor.Application.Features.Logs.DTOs;
using CleanArchitecture.Blazor.Domain.Entities.Log;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Blazor.Application.Features.Logs.Queries.Export
{
    public class ExportLogsQuery : IRequest<byte[]>
    {
        public string filterRules { get; set; }
        public string sort { get; set; } = "Id";
        public string order { get; set; } = "desc";
    }

    public class ExportLogsQueryHandler :
         IRequestHandler<ExportLogsQuery, byte[]>
    {
        private readonly IApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IExcelService excelService;
        private readonly IStringLocalizer<ExportLogsQueryHandler> localizer;

        public ExportLogsQueryHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IExcelService excelService,
            IStringLocalizer<ExportLogsQueryHandler> localizer)
        {
            this.context = context;
            this.mapper = mapper;
            this.excelService = excelService;
            this.localizer = localizer;
        }

        public async Task<byte[]> Handle(ExportLogsQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Logger, bool>> filters = PredicateBuilder.FromFilter<Logger>(request.filterRules);
            List<LogDto> data = await context.Loggers
                .Where(filters)
                //.OrderBy($"{request.sort} {request.order}")
                .ProjectTo<LogDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            byte[] result = await excelService.ExportAsync(
               data: data,
               mappers: new Dictionary<string, Func<LogDto, object>>()
                {
                    //{ _localizer["Id"], item => item.Id },
                    { localizer["Time Stamp"], item => item.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss") },
                    { localizer["Level"], item => item.Level },
                    { localizer["Message"], item => item.Message },
                    { localizer["Exception"], item => item.Exception },
                    { localizer["User Name"], item => item.UserName },
                    { localizer["Message Template"], item => item.MessageTemplate },
                    { localizer["Properties"], item => item.Properties },
                },
               sheetName: localizer["Logs"]);
            return result;
        }
    }
}

