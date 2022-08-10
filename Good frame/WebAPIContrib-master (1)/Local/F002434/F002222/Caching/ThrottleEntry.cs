using System;
using System.Collections.Generic;
using System.Text;

namespace F002222.Caching
{
    /// <summary>
    /// 节流阀。包含请求次数和请求开始时间
    /// </summary>
    public class ThrottleEntry
    {
        public DateTime PeriodStart { get; set; }
        public long Requests { get; set; }

        public ThrottleEntry()
        {
            PeriodStart = DateTime.UtcNow;
            Requests = 0;
        }
    }
}
