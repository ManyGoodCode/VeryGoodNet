using System;

namespace CleanArchitecture.Blazor.Application.Features.Logs.DTOs
{
    public class LogLevelChartDto
    {
        public string level { get; set; }
        public int total { get; set; }
    }

    public class LogTimeLineDto
    {
        public DateTime time { get; set; }
        public int total { get; set; }
        public string level { get; set; }
    }
}
