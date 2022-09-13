using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.Logs.DTOs;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Linq;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Application.Features.Logs.Queries.ChatData
{
    public class LogsTimeLineChatDataQuery : IRequest<IEnumerable<LogTimeLineDto>>
    {
        public DateTime LastDateTime { get; set; } = DateTime.Now.AddDays(-3);
    }

    public class LogsLevelChatDataQuery : IRequest<IEnumerable<LogLevelChartDto>>
    {
        public DateTime LastDateTime { get; set; } = DateTime.Now.AddDays(-3);
    }

    public class LogsChatDataQueryHandler :
         IRequestHandler<LogsTimeLineChatDataQuery, IEnumerable<LogTimeLineDto>>,
         IRequestHandler<LogsLevelChatDataQuery, IEnumerable<LogLevelChartDto>>
    {
        private readonly IApplicationDbContext context;
        private readonly IStringLocalizer<LogsChatDataQueryHandler> localizer;

        public LogsChatDataQueryHandler(
            IApplicationDbContext context,
            IStringLocalizer<LogsChatDataQueryHandler> localizer)
        {
            this.context = context;
            this.localizer = localizer;
        }

        public async Task<IEnumerable<LogTimeLineDto>> Handle(LogsTimeLineChatDataQuery request, CancellationToken cancellationToken)
        {
            string[] levels = new string[] { "Information", "Trace", "Debug", "Warning", "Error", "Fatal" };
            var data = await context.Loggers.Where(x => x.TimeStamp >= request.LastDateTime)
                      .GroupBy(x => new { x.Level, x.TimeStamp.Date, x.TimeStamp.Hour })
                      .Select(x => new { x.Key.Level, x.Key.Date, x.Key.Hour, Total = x.Count() })
                      .OrderBy(x => x.Level).ThenBy(x => x.Date)
                      .ToListAsync(cancellationToken);
            IEnumerable<LogTimeLineDto> result = data.Select(item => new LogTimeLineDto()
            {
                time = item.Date.AddHours(item.Hour),
                level = item.Level,
                total = item.Total
            }).OrderBy(x => x.level).ThenBy(x => x.time);

            return result;
        }

        public async Task<IEnumerable<LogLevelChartDto>> Handle(LogsLevelChatDataQuery request, CancellationToken cancellationToken)
        {
            string[] levels = new string[] { "Information", "Trace", "Debug", "Warning", "Error", "Fatal" };
            IEnumerable<LogLevelChartDto> data = await context.Loggers.Where(x => x.TimeStamp >= request.LastDateTime)
                      .GroupBy(x => new { x.Level })
                      .Select(x => new LogLevelChartDto() { level = x.Key.Level, total = x.Count() })
                      .OrderBy(x => x.level)
                      .ToListAsync(cancellationToken);
            return data;
        }
    }
}