
namespace OxyPlot.Axes
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    
    public class DateTimeAxis : LinearAxis
    {
        private static readonly DateTime TimeOrigin = new DateTime(1899, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        private static readonly double MaxDayValue = (DateTime.MaxValue - TimeOrigin).TotalDays;
        private static readonly double MinDayValue = (DateTime.MinValue - TimeOrigin).TotalDays;
        private DateTimeIntervalType actualIntervalType;
        private DateTimeIntervalType actualMinorIntervalType;
        public DateTimeAxis()
        {
            this.Position = AxisPosition.Bottom;
            this.IntervalType = DateTimeIntervalType.Auto;
            this.FirstDayOfWeek = DayOfWeek.Monday;
            this.CalendarWeekRule = CalendarWeekRule.FirstFourDayWeek;
        }

        public CalendarWeekRule CalendarWeekRule { get; set; }
        public DayOfWeek FirstDayOfWeek { get; set; }
        public DateTimeIntervalType IntervalType { get; set; }
        public DateTimeIntervalType MinorIntervalType { get; set; }
        public TimeZoneInfo TimeZone { get; set; }
        public static DataPoint CreateDataPoint(DateTime x, double y)
        {
            return new DataPoint(ToDouble(x), y);
        }

        public static DataPoint CreateDataPoint(DateTime x, DateTime y)
        {
            return new DataPoint(ToDouble(x), ToDouble(y));
        }

        public static DataPoint CreateDataPoint(double x, DateTime y)
        {
            return new DataPoint(x, ToDouble(y));
        }

        public static DateTime ToDateTime(double value)
        {
            if (double.IsNaN(value) || value < MinDayValue || value > MaxDayValue)
            {
                return new DateTime();
            }

            return TimeOrigin.AddDays(value - 1);
        }

        public static double ToDouble(DateTime value)
        {
            var span = value - TimeOrigin;
            return span.TotalDays + 1;
        }

        public override void GetTickValues(
            out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
        {
            minorTickValues = this.CreateDateTimeTickValues(
                this.ClipMinimum, this.ClipMaximum, this.ActualMinorStep, this.actualMinorIntervalType);
            majorTickValues = this.CreateDateTimeTickValues(
                this.ClipMinimum, this.ClipMaximum, this.ActualMajorStep, this.actualIntervalType);
            majorLabelValues = majorTickValues;

            minorTickValues = AxisUtilities.FilterRedundantMinorTicks(majorTickValues, minorTickValues);
        }

        public override object GetValue(double x)
        {
            var time = ToDateTime(x);

            if (this.TimeZone != null)
            {
                time = TimeZoneInfo.ConvertTime(time, this.TimeZone);
            }

            return time;
        }
        
        internal override void UpdateIntervals(OxyRect plotArea)
        {
            base.UpdateIntervals(plotArea);
            switch (this.actualIntervalType)
            {
                case DateTimeIntervalType.Years:
                    this.ActualMinorStep = 31;
                    this.actualMinorIntervalType = DateTimeIntervalType.Years;
                    if (this.StringFormat == null)
                    {
                        this.ActualStringFormat = "yyyy";
                    }

                    break;
                case DateTimeIntervalType.Months:
                    this.actualMinorIntervalType = DateTimeIntervalType.Months;
                    if (this.StringFormat == null)
                    {
                        this.ActualStringFormat = "yyyy-MM-dd";
                    }

                    break;
                case DateTimeIntervalType.Weeks:
                    this.actualMinorIntervalType = DateTimeIntervalType.Days;
                    this.ActualMajorStep = 7;
                    this.ActualMinorStep = 1;
                    if (this.StringFormat == null)
                    {
                        this.ActualStringFormat = "yyyy/ww";
                    }

                    break;
                case DateTimeIntervalType.Days:
                    this.ActualMinorStep = this.ActualMajorStep;
                    if (this.StringFormat == null)
                    {
                        this.ActualStringFormat = "yyyy-MM-dd";
                    }

                    break;
                case DateTimeIntervalType.Hours:
                    this.ActualMinorStep = this.ActualMajorStep;
                    if (this.StringFormat == null)
                    {
                        this.ActualStringFormat = "HH:mm";
                    }

                    break;
                case DateTimeIntervalType.Minutes:
                    this.ActualMinorStep = this.ActualMajorStep;
                    if (this.StringFormat == null)
                    {
                        this.ActualStringFormat = "HH:mm";
                    }

                    break;
                case DateTimeIntervalType.Seconds:
                    this.ActualMinorStep = this.ActualMajorStep;
                    if (this.StringFormat == null)
                    {
                        this.ActualStringFormat = "HH:mm:ss";
                    }

                    break;
                    
                    
                    
                case DateTimeIntervalType.Milliseconds:
                    this.ActualMinorStep = this.ActualMajorStep;
                    if (this.ActualStringFormat == null)
                    {
                        this.ActualStringFormat = "HH:mm:ss.fff";
                    }

                    break;
                    
                case DateTimeIntervalType.Manual:
                    break;
                case DateTimeIntervalType.Auto:
                    break;
            }
        }

        protected override string GetDefaultStringFormat()
        {
            return null;
        }

        protected override string FormatValueOverride(double x)
        {
            // convert the double value to a DateTime
            var time = ToDateTime(x);

            // If a time zone is specified, convert the time
            if (this.TimeZone != null)
            {
                time = TimeZoneInfo.ConvertTime(time, this.TimeZone);
            }

            string fmt = this.ActualStringFormat;
            if (fmt == null)
            {
                return time.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            }

            int week = this.GetWeek(time);
            fmt = fmt.Replace("ww", week.ToString("00"));
            fmt = fmt.Replace("w", week.ToString(CultureInfo.InvariantCulture));
            fmt = string.Concat("{0:", fmt, "}");
            return string.Format(this.ActualCulture, fmt, time);
        }

        protected override double CalculateActualInterval(double availableSize, double maxIntervalSize)
        {
            const double Year = 365.25;
            const double Month = 30.5;
            const double Week = 7;
            const double Day = 1.0;
            const double Hour = Day / 24;
            const double Minute = Hour / 60;
            const double Second = Minute / 60;
            const double MilliSecond = Second / 1000;

            double range = Math.Abs(this.ClipMinimum - this.ClipMaximum);

            var goodIntervals = new[]
                                    {   MilliSecond, 2 * MilliSecond, 10 * MilliSecond, 100 * MilliSecond,
                                        Second, 2 * Second, 5 * Second, 10 * Second, 30 * Second, Minute, 2 * Minute,
                                        5 * Minute, 10 * Minute, 30 * Minute, Hour, 4 * Hour, 8 * Hour, 12 * Hour, Day,
                                        2 * Day, 5 * Day, Week, 2 * Week, Month, 2 * Month, 3 * Month, 4 * Month,
                                        6 * Month, Year
                                    };

            double interval = goodIntervals[0];

            int maxNumberOfIntervals = Math.Max((int)(availableSize / maxIntervalSize), 2);

            while (true)
            {
                if (range / interval < maxNumberOfIntervals)
                {
                    break;
                }

                double nextInterval = goodIntervals.FirstOrDefault(i => i > interval);
                if (Math.Abs(nextInterval) <= double.Epsilon)
                {
                    nextInterval = interval * 2;
                }

                interval = nextInterval;
            }

            this.actualIntervalType = this.IntervalType;
            this.actualMinorIntervalType = this.MinorIntervalType;

            if (this.IntervalType == DateTimeIntervalType.Auto)
            {
                this.actualIntervalType = DateTimeIntervalType.Milliseconds;

                if (interval >= 1.0 / 24 / 60 / 60)
                {
                    this.actualIntervalType = DateTimeIntervalType.Seconds;
                }
                    
                if (interval >= 1.0 / 24 / 60)
                {
                    this.actualIntervalType = DateTimeIntervalType.Minutes;
                }

                if (interval >= 1.0 / 24)
                {
                    this.actualIntervalType = DateTimeIntervalType.Hours;
                }

                if (interval >= 1)
                {
                    this.actualIntervalType = DateTimeIntervalType.Days;
                }

                if (interval >= 30)
                {
                    this.actualIntervalType = DateTimeIntervalType.Months;
                }

                if (range >= 365.25)
                {
                    this.actualIntervalType = DateTimeIntervalType.Years;
                }
            }

            if (this.actualIntervalType == DateTimeIntervalType.Months)
            {
                double monthsRange = range / 30.5;
                interval = this.CalculateActualInterval(availableSize, maxIntervalSize, monthsRange);
            }

            if (this.actualIntervalType == DateTimeIntervalType.Years)
            {
                double yearsRange = range / 365.25;
                interval = this.CalculateActualInterval(availableSize, maxIntervalSize, yearsRange);
            }

            if (this.actualMinorIntervalType == DateTimeIntervalType.Auto)
            {
                switch (this.actualIntervalType)
                {
                    case DateTimeIntervalType.Years:
                        this.actualMinorIntervalType = DateTimeIntervalType.Months;
                        break;
                    case DateTimeIntervalType.Months:
                        this.actualMinorIntervalType = DateTimeIntervalType.Days;
                        break;
                    case DateTimeIntervalType.Weeks:
                        this.actualMinorIntervalType = DateTimeIntervalType.Days;
                        break;
                    case DateTimeIntervalType.Days:
                        this.actualMinorIntervalType = DateTimeIntervalType.Hours;
                        break;
                    case DateTimeIntervalType.Hours:
                        this.actualMinorIntervalType = DateTimeIntervalType.Minutes;
                        break;
                    default:
                        this.actualMinorIntervalType = DateTimeIntervalType.Days;
                        break;
                }
            }

            return interval;
        }

        private IList<double> CreateDateTickValues(
            double min, double max, double step, DateTimeIntervalType intervalType)
        {
            var values = new Collection<double>();
            var start = ToDateTime(min);
            if (start.Ticks == 0)
            {
                // Invalid start time
                return values;
            }

            switch (intervalType)
            {
                case DateTimeIntervalType.Weeks:

                    // make sure the first tick is at the 1st day of a week
                    start = start.AddDays(-(int)start.DayOfWeek + (int)this.FirstDayOfWeek);
                    break;
                case DateTimeIntervalType.Months:

                    // make sure the first tick is at the 1st of a month
                    start = new DateTime(start.Year, start.Month, 1);
                    break;
                case DateTimeIntervalType.Years:

                    // make sure the first tick is at Jan 1st
                    start = new DateTime(start.Year, 1, 1);
                    break;
            }

            // Adds a tick to the end time to make sure the end DateTime is included.
            var end = ToDateTime(max).AddTicks(1);
            if (end.Ticks == 0)
            {
                // Invalid end time
                return values;
            }

            var current = start;
            double eps = step * 1e-3;
            var minDateTime = ToDateTime(min - eps);
            var maxDateTime = ToDateTime(max + eps);

            if (minDateTime.Ticks == 0 || maxDateTime.Ticks == 0)
            {
                // Invalid min/max time
                return values;
            }

            while (current < end)
            {
                if (current > minDateTime && current < maxDateTime)
                {
                    values.Add(ToDouble(current));
                }

                try
                {
                    switch (intervalType)
                    {
                        case DateTimeIntervalType.Months:
                            current = current.AddMonths((int)Math.Ceiling(step));
                            break;
                        case DateTimeIntervalType.Years:
                            current = current.AddYears((int)Math.Ceiling(step));
                            break;
                        default:
                            current = current.AddDays(step);
                            break;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    // AddMonths/AddYears/AddDays can throw an exception
                    // We could test this by comparing to MaxDayValue/MinDayValue, but it is easier to catch the exception...
                    break;
                }
            }

            return values;
        }

        private IList<double> CreateDateTimeTickValues(
            double min, double max, double interval, DateTimeIntervalType intervalType)
        {
            // If the step size is more than 7 days (e.g. months or years) we use a specialized tick generation method that adds tick values with uneven spacing...
            if (intervalType > DateTimeIntervalType.Days)
            {
                return this.CreateDateTickValues(min, max, interval, intervalType);
            }

            // For shorter step sizes we use the method from Axis
            return this.CreateTickValues(min, max, interval);
        }
        
        private int GetWeek(DateTime date)
        {
            return this.ActualCulture.Calendar.GetWeekOfYear(date, this.CalendarWeekRule, this.FirstDayOfWeek);
        }
    }
}
