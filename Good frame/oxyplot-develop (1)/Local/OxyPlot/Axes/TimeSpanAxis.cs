
namespace OxyPlot.Axes
{
    using System;
    using System.Linq;

    public class TimeSpanAxis : LinearAxis
    {
        public static double ToDouble(TimeSpan s)
        {
            return s.TotalSeconds;
        }

        public static TimeSpan ToTimeSpan(double value)
        {
            return TimeSpan.FromSeconds(value);
        }
        
        public override object GetValue(double x)
        {
            return TimeSpan.FromSeconds(x);
        }

        /// <summary>
        /// Gets the default format string.
        /// </summary>
        /// <returns>
        /// The default format string.
        /// </returns>
        protected override string GetDefaultStringFormat()
        {
            return null;
        }

        protected override string FormatValueOverride(double x)
        {
            var span = ToTimeSpan(x);

            var fmt = this.ActualStringFormat ?? this.StringFormat ?? string.Empty;
            fmt = fmt.Replace(":", "\\:");
            fmt = string.Concat("{0:", fmt, "}");

            return string.Format(this.ActualCulture, fmt, span);
        }

        protected override double CalculateActualInterval(double availableSize, double maxIntervalSize)
        {
            double range = Math.Abs(this.ClipMinimum - this.ClipMaximum);
            double interval = 1;
            var goodIntervals = new[] { 1.0, 5, 10, 30, 60, 120, 300, 600, 900, 1200, 1800, 3600 };

            int maxNumberOfIntervals = Math.Max((int)(availableSize / maxIntervalSize), 2);

            while (true)
            {
                if (range / interval < maxNumberOfIntervals)
                {
                    return interval;
                }

                double nextInterval = goodIntervals.FirstOrDefault(i => i > interval);
                if (nextInterval == default(double))
                {
                    nextInterval = interval * 2;
                }

                interval = nextInterval;
            }
        }
    }
}
