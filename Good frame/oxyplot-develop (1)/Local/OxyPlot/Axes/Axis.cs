
namespace OxyPlot.Axes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OxyPlot.Series;
    using OxyPlot.Utilities;

    public abstract class Axis : PlotElement
    {
        protected static readonly Func<double, double> Exponent = x => Math.Floor(ThresholdRound(Math.Log(Math.Abs(x), 10)));
        protected static readonly Func<double, double> Mantissa = x => ThresholdRound(x / Math.Pow(10, Exponent(x)));
        protected static readonly Func<double, double> ThresholdRound = x => Math.Abs(Math.Round(x) - x) < 1e-6 ? Math.Round(x) : x;

        private double offset;
        private double scale;
        private AxisPosition position;
        protected Axis()
        {
            this.Position = AxisPosition.Left;
            this.PositionTier = 0;
            this.IsAxisVisible = true;
            this.Layer = AxisLayer.BelowSeries;

            this.ViewMaximum = double.NaN;
            this.ViewMinimum = double.NaN;

            this.AbsoluteMaximum = double.MaxValue;
            this.AbsoluteMinimum = double.MinValue;

            this.Minimum = double.NaN;
            this.Maximum = double.NaN;
            this.MinorStep = double.NaN;
            this.MajorStep = double.NaN;
            this.MinimumMinorStep = 0;
            this.MinimumMajorStep = 0;

            this.MinimumPadding = 0.01;
            this.MaximumPadding = 0.01;
            this.MinimumRange = 0;
            this.MaximumRange = double.PositiveInfinity;
            this.MinimumDataMargin = 0;
            this.MaximumDataMargin = 0;
            this.MinimumMargin = 0;
            this.MaximumMargin = 0;

            this.TickStyle = TickStyle.Outside;
            this.TicklineColor = OxyColors.Black;
            this.MinorTicklineColor = OxyColors.Automatic;

            this.AxislineStyle = LineStyle.None;
            this.AxislineColor = OxyColors.Black;
            this.AxislineThickness = 1.0;

            this.MajorGridlineStyle = LineStyle.None;
            this.MajorGridlineColor = OxyColor.FromArgb(0x40, 0, 0, 0);
            this.MajorGridlineThickness = 1;

            this.MinorGridlineStyle = LineStyle.None;
            this.MinorGridlineColor = OxyColor.FromArgb(0x20, 0, 0, 0x00);
            this.MinorGridlineThickness = 1;

            this.ExtraGridlineStyle = LineStyle.Solid;
            this.ExtraGridlineColor = OxyColors.Black;
            this.ExtraGridlineThickness = 1;

            this.MinorTickSize = 4;
            this.MajorTickSize = 7;

            this.StartPosition = 0;
            this.EndPosition = 1;

            this.TitlePosition = 0.5;
            this.TitleFormatString = "{0} [{1}]";
            this.TitleClippingLength = 0.9;
            this.TitleColor = OxyColors.Automatic;
            this.TitleFontSize = double.NaN;
            this.TitleFontWeight = FontWeights.Normal;
            this.ClipTitle = true;

            this.Angle = 0;

            this.IsZoomEnabled = true;
            this.IsPanEnabled = true;

            this.FilterMinValue = double.MinValue;
            this.FilterMaxValue = double.MaxValue;
            this.FilterFunction = null;

            this.IntervalLength = 60;

            this.AxisDistance = 0;
            this.AxisTitleDistance = 4;
            this.AxisTickToLabelDistance = 4;

            this.DataMaximum = double.NaN;
            this.DataMinimum = double.NaN;
        }

        [Obsolete("May be removed in v4.0 (#111)")]
        public event EventHandler<AxisChangedEventArgs> AxisChanged;

        [Obsolete("May be removed in v4.0 (#111)")]
        public event EventHandler TransformChanged;

        public double AbsoluteMaximum { get; set; }
        public double AbsoluteMinimum { get; set; }
        public double ActualMajorStep { get; protected set; }
        public double ActualMaximum { get; protected set; }
        public double ActualMinimum { get; protected set; }
        public double ClipMaximum { get; protected set; }
        public double ClipMinimum { get; protected set; }
        public double ActualMinorStep { get; protected set; }
        public string ActualStringFormat { get; protected set; }
        public string ActualTitle
        {
            get
            {
                if (this.Unit != null)
                {
                    return string.Format(this.TitleFormatString, this.Title, this.Unit);
                }

                return this.Title;
            }
        }

        public double Angle { get; set; }
        public double AxisTickToLabelDistance { get; set; }
        public double AxisTitleDistance { get; set; }
        public double AxisDistance { get; set; }
        public OxyColor AxislineColor { get; set; }
        public LineStyle AxislineStyle { get; set; }
        public double AxislineThickness { get; set; }
        public bool ClipTitle { get; set; }
        public bool CropGridlines { get; set; }
        public double DataMaximum { get; protected set; }
        public double DataMinimum { get; protected set; }
        public double EndPosition { get; set; }
        public OxyColor ExtraGridlineColor { get; set; }
        public LineStyle ExtraGridlineStyle { get; set; }
        public double ExtraGridlineThickness { get; set; }
        public double[] ExtraGridlines { get; set; }
        public Func<double, bool> FilterFunction { get; set; }
        public double FilterMaxValue { get; set; }
        public double FilterMinValue { get; set; }
        public double IntervalLength { get; set; }
        public bool IsAxisVisible { get; set; }
        public bool IsPanEnabled { get; set; }
        public bool IsReversed
        {
            get
            {
                return this.StartPosition > this.EndPosition;
            }
        }
        
        public bool IsZoomEnabled { get; set; }
        public string Key { get; set; }
        public Func<double, string> LabelFormatter { get; set; }
        public AxisLayer Layer { get; set; }
        public OxyColor MajorGridlineColor { get; set; }
        public LineStyle MajorGridlineStyle { get; set; }
        public double MajorGridlineThickness { get; set; }
        public double MajorStep { get; set; }
        public double MajorTickSize { get; set; }
        public double Maximum { get; set; }
        public double MaximumPadding { get; set; }
        public double MaximumDataMargin { get; set; }
        public double MaximumMargin { get; set; }
        public double MaximumRange { get; set; }
        public double Minimum { get; set; }
        public double MinimumMajorStep { get; set; }
        public double MinimumMinorStep { get; set; }
        public double MinimumPadding { get; set; }
        public double MinimumDataMargin { get; set; }
        public double MinimumMargin { get; set; }
        public double MinimumRange { get; set; }
        public OxyColor MinorGridlineColor { get; set; }
        public LineStyle MinorGridlineStyle { get; set; }
        public double MinorGridlineThickness { get; set; }
        public double MinorStep { get; set; }
        public OxyColor MinorTicklineColor { get; set; }
        public double MinorTickSize { get; set; }
        public double Offset
        {
            get
            {
                return this.offset;
            }
        }

        public AxisPosition Position
        {
            get
            {
                return this.position;
            }

            set
            {
                this.position = value;
            }
        }

        public bool PositionAtZeroCrossing { get; set; }
        public int PositionTier { get; set; }
        public double Scale
        {
            get
            {
                return this.scale;
            }
        }

        public ScreenPoint ScreenMax { get; protected set; }
        public ScreenPoint ScreenMin { get; protected set; }
        public double StartPosition { get; set; }
        public string StringFormat { get; set; }
        public TickStyle TickStyle { get; set; }
        public OxyColor TicklineColor { get; set; }
        public string Title { get; set; }
        public double TitleClippingLength { get; set; }
        public OxyColor TitleColor { get; set; }
        public string TitleFont { get; set; }
        public double TitleFontSize { get; set; }
        public double TitleFontWeight { get; set; }
        public string TitleFormatString { get; set; }
        public double TitlePosition { get; set; }
        public string Unit { get; set; }
        public bool UseSuperExponentialFormat { get; set; }

        public OxyThickness DesiredMargin { get; protected set; }
        internal double PositionTierMaxShift { get; set; }
        internal double PositionTierMinShift { get; set; }
        internal double PositionTierSize { get; set; }
        protected internal OxyColor ActualTitleColor
        {
            get
            {
                return this.TitleColor.GetActualColor(this.PlotModel.TextColor);
            }
        }

        protected internal string ActualTitleFont
        {
            get
            {
                return this.TitleFont ?? this.PlotModel.DefaultFont;
            }
        }

        protected internal double ActualTitleFontSize
        {
            get
            {
                return !double.IsNaN(this.TitleFontSize) ? this.TitleFontSize : this.ActualFontSize;
            }
        }

        protected internal double ActualTitleFontWeight
        {
            get
            {
                return !double.IsNaN(this.TitleFontWeight) ? this.TitleFontWeight : this.ActualFontWeight;
            }
        }

        protected double ViewMaximum { get; set; }
        protected double ViewMinimum { get; set; }
        public static double ToDouble(object value)
        {
            if (value is DateTime)
            {
                return DateTimeAxis.ToDouble((DateTime)value);
            }

            if (value is TimeSpan)
            {
                return TimeSpanAxis.ToDouble((TimeSpan)value);
            }

            return Convert.ToDouble(value);
        }

        public static DataPoint InverseTransform(ScreenPoint p, Axis xaxis, Axis yaxis)
        {
            return xaxis.InverseTransform(p.x, p.y, yaxis);
        }

        public string FormatValue(double x)
        {
            if (this.LabelFormatter != null)
            {
                return this.LabelFormatter(x);
            }

            return this.FormatValueOverride(x);
        }

        public virtual void GetTickValues(
            out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
        {
            minorTickValues = this.CreateTickValues(this.ClipMinimum, this.ClipMaximum, this.ActualMinorStep);
            majorTickValues = this.CreateTickValues(this.ClipMinimum, this.ClipMaximum, this.ActualMajorStep);
            majorLabelValues = majorTickValues;

            minorTickValues = AxisUtilities.FilterRedundantMinorTicks(majorTickValues, minorTickValues);
        }
        
        public virtual object GetValue(double x)
        {
            return x;
        }

        public virtual DataPoint InverseTransform(double x, double y, Axis yaxis)
        {
            return new DataPoint(this.InverseTransform(x), yaxis != null ? yaxis.InverseTransform(y) : 0);
        }

        public virtual double InverseTransform(double sx)
        {
            return (sx / this.scale) + this.offset;
        }

        public bool IsHorizontal()
        {
            return this.position == AxisPosition.Top || this.position == AxisPosition.Bottom;
        }

        public bool IsValidValue(double value)
        {
#pragma warning disable 1718
            // ReSharper disable EqualExpressionComparison
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return value == value &&
                value != 1.0 / 0.0 &&
                value != -1.0 / 0.0 &&
                value < this.FilterMaxValue &&
                value > this.FilterMinValue &&
                (this.FilterFunction == null || this.FilterFunction(value));
            // ReSharper restore CompareOfFloatsByEqualityOperator
            // ReSharper restore EqualExpressionComparison
#pragma warning restore 1718
        }


        public bool IsVertical()
        {
            return this.position == AxisPosition.Left || this.position == AxisPosition.Right;
        }

        public abstract bool IsXyAxis();
        public virtual bool IsLogarithmic()
        {
            return false;
        }

        public virtual void Measure(IRenderContext rc)
        {
            if (this.Position == AxisPosition.None)
            {
                this.DesiredMargin = new OxyThickness(0);
                return;
            }

            this.GetTickValues(out var majorLabelValues, out _, out _);

            var maximumTextSize = new OxySize();
            foreach (var v in majorLabelValues)
            {
                var s = this.FormatValue(v);
                var size = rc.MeasureText(s, this.ActualFont, this.ActualFontSize, this.ActualFontWeight, this.Angle);
                maximumTextSize = maximumTextSize.Include(size);
            }

            var titleTextSize = rc.MeasureText(this.ActualTitle, this.ActualTitleFont, this.ActualTitleFontSize, this.ActualTitleFontWeight);

            var marginLeft = 0d;
            var marginTop = 0d;
            var marginRight = 0d;
            var marginBottom = 0d;

            var minOuterMargin = Math.Max(0, this.IsReversed ? this.MaximumMargin : this.MinimumMargin);
            var maxOuterMargin = Math.Max(0, this.IsReversed ? this.MinimumMargin : this.MaximumMargin);

            var margin = this.TickStyle switch
            {
                TickStyle.Outside => this.MajorTickSize,
                TickStyle.Crossing => this.MajorTickSize * 0.75,
                _ => 0
            };

            margin += this.AxisDistance + this.AxisTickToLabelDistance;

            if (titleTextSize.Height > 0)
            {
                margin += this.AxisTitleDistance + titleTextSize.Height;
            }

            switch (this.Position)
            {
                case AxisPosition.Left:
                    marginLeft = margin + maximumTextSize.Width;
                    break;
                case AxisPosition.Right:
                    marginRight = margin + maximumTextSize.Width;
                    break;
                case AxisPosition.Top:
                    marginTop = margin + maximumTextSize.Height;
                    break;
                case AxisPosition.Bottom:
                    marginBottom = margin + maximumTextSize.Height;
                    break;
                case AxisPosition.All:
                    marginLeft = marginRight = margin + maximumTextSize.Width;
                    marginTop = marginBottom = margin + maximumTextSize.Height;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (this.IsPanEnabled || this.IsZoomEnabled)
            {
                var reachesMinPosition = Math.Min(this.StartPosition, this.EndPosition) < 0.01;
                var reachesMaxPosition = Math.Max(this.StartPosition, this.EndPosition) > 0.99;

                switch (this.Position)
                {
                    case AxisPosition.Left:
                    case AxisPosition.Right:
                        if (reachesMinPosition)
                        {
                            marginBottom = Math.Max(0, (maximumTextSize.Height / 2) - minOuterMargin);
                        }

                        if (reachesMaxPosition)
                        {
                            marginTop = Math.Max(0, (maximumTextSize.Height / 2) - maxOuterMargin);
                        }

                        break;
                    case AxisPosition.Top:
                    case AxisPosition.Bottom:
                        if (reachesMinPosition)
                        {
                            marginLeft = Math.Max(0, (maximumTextSize.Width / 2) - minOuterMargin);
                        }

                        if (reachesMaxPosition)
                        {
                            marginRight = Math.Max(0, (maximumTextSize.Width / 2) - maxOuterMargin);
                        }

                        break;
                }
            }
            else if (majorLabelValues.Count > 0)
            {
                var minLabel = majorLabelValues.Min();
                var maxLabel = majorLabelValues.Max();

                var minLabelText = this.FormatValue(minLabel);
                var maxLabelText = this.FormatValue(maxLabel);

                var minLabelSize = rc.MeasureText(minLabelText, this.ActualFont, this.ActualFontSize, this.ActualFontWeight, this.Angle);
                var maxLabelSize = rc.MeasureText(maxLabelText, this.ActualFont, this.ActualFontSize, this.ActualFontWeight, this.Angle);

                var minLabelPosition = this.Transform(minLabel);
                var maxLabelPosition = this.Transform(maxLabel);

                if (minLabelPosition > maxLabelPosition)
                {
                    Helpers.Swap(ref minLabelPosition, ref maxLabelPosition);
                    Helpers.Swap(ref minLabelSize, ref maxLabelSize);
                }

                switch (this.Position)
                {
                    case AxisPosition.Left:
                    case AxisPosition.Right:
                        var screenMinY = Math.Min(this.ScreenMin.Y, this.ScreenMax.Y);
                        var screenMaxY = Math.Max(this.ScreenMin.Y, this.ScreenMax.Y);

                        marginTop = Math.Max(0, screenMinY - minLabelPosition + (minLabelSize.Height / 2) - minOuterMargin);
                        marginBottom = Math.Max(0, maxLabelPosition - screenMaxY + (maxLabelSize.Height / 2) - maxOuterMargin);
                        break;
                    case AxisPosition.Top:
                    case AxisPosition.Bottom:
                        var screenMinX = Math.Min(this.ScreenMin.X, this.ScreenMax.X);
                        var screenMaxX = Math.Max(this.ScreenMin.X, this.ScreenMax.X);

                        marginLeft = Math.Max(0, screenMinX - minLabelPosition + (minLabelSize.Width / 2) - minOuterMargin);
                        marginRight = Math.Max(0, maxLabelPosition - screenMaxX + (maxLabelSize.Width / 2) - maxOuterMargin);
                        break;
                }
            }

            this.DesiredMargin = new OxyThickness(marginLeft, marginTop, marginRight, marginBottom);
        }

        public virtual void Pan(ScreenPoint ppt, ScreenPoint cpt)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }

            bool isHorizontal = this.IsHorizontal();

            double dsx = isHorizontal ? cpt.X - ppt.X : cpt.Y - ppt.Y;
            this.Pan(dsx);
        }

        public virtual void Pan(double delta)
        {
            if (!this.IsPanEnabled)
            {
                return;
            }

            var oldMinimum = this.ActualMinimum;
            var oldMaximum = this.ActualMaximum;

            double dx = delta / this.Scale;

            double newMinimum = this.ActualMinimum - dx;
            double newMaximum = this.ActualMaximum - dx;
            if (newMinimum < this.AbsoluteMinimum)
            {
                newMinimum = this.AbsoluteMinimum;
                newMaximum = Math.Min(newMinimum + this.ActualMaximum - this.ActualMinimum, this.AbsoluteMaximum);
            }

            if (newMaximum > this.AbsoluteMaximum)
            {
                newMaximum = this.AbsoluteMaximum;
                newMinimum = Math.Max(newMaximum - (this.ActualMaximum - this.ActualMinimum), this.AbsoluteMinimum);
            }

            this.ViewMinimum = newMinimum;
            this.ViewMaximum = newMaximum;
            this.UpdateActualMaxMin();

            var deltaMinimum = this.ActualMinimum - oldMinimum;
            var deltaMaximum = this.ActualMaximum - oldMaximum;

            this.OnAxisChanged(new AxisChangedEventArgs(AxisChangeTypes.Pan, deltaMinimum, deltaMaximum));
        }

        public virtual void Render(IRenderContext rc, int pass)
        {
            if (this.Position == AxisPosition.None)
            {
                return;
            }

            var r = new HorizontalAndVerticalAxisRenderer(rc, this.PlotModel);
            r.Render(this, pass);
        }

        public virtual void Reset()
        {
            var oldMinimum = this.ActualMinimum;
            var oldMaximum = this.ActualMaximum;

            this.ViewMinimum = double.NaN;
            this.ViewMaximum = double.NaN;
            this.UpdateActualMaxMin();

            var deltaMinimum = this.ActualMinimum - oldMinimum;
            var deltaMaximum = this.ActualMaximum - oldMaximum;

            this.OnAxisChanged(new AxisChangedEventArgs(AxisChangeTypes.Reset, deltaMinimum, deltaMaximum));
        }

        public override string ToString()
        {
            return string.Format(
                this.ActualCulture,
                "{0}({1}, {2}, {3}, {4})",
                this.GetType().Name,
                this.Position,
                this.ClipMinimum,
                this.ClipMaximum,
                this.ActualMajorStep);
        }

        public virtual ScreenPoint Transform(double x, double y, Axis yaxis)
        {
            if (yaxis == null)
            {
                throw new NullReferenceException("Y axis should not be null when transforming.");
            }

            return new ScreenPoint(this.Transform(x), yaxis.Transform(y));
        }

        public virtual double Transform(double x)
        {
#if DEBUG
            // check if the screen coordinate is very big, this could cause issues
            // only do this in DEBUG builds, as it affects performance
            var s = (x - this.offset) * this.scale;
            if (s * s > 1e12)
            {
                throw new InvalidOperationException($"Invalid transform (screen coordinate={s}). This could cause issues with the presentation framework.");
            }

            return s;
#else
            return (x - this.offset) * this.scale;
#endif
        }

        public virtual void Zoom(double newScale)
        {
            var oldMinimum = this.ActualMinimum;
            var oldMaximum = this.ActualMaximum;

            double sx1 = this.Transform(this.ActualMaximum);
            double sx0 = this.Transform(this.ActualMinimum);

            double sgn = Math.Sign(this.scale);
            double mid = (this.PreTransform(this.ActualMaximum) + this.PreTransform(this.ActualMinimum)) / 2;


            double dx = (this.offset - mid) * this.scale;
            var newOffset = (dx / (sgn * newScale)) + mid;
            this.SetTransform(sgn * newScale, newOffset);

            double newMaximum = this.InverseTransform(sx1);
            double newMinimum = this.InverseTransform(sx0);

            if (newMinimum < this.AbsoluteMinimum && newMaximum > this.AbsoluteMaximum)
            {
                newMinimum = this.AbsoluteMinimum;
                newMaximum = this.AbsoluteMaximum;
            }
            else
            {
                if (newMinimum < this.AbsoluteMinimum)
                {
                    double d = newMaximum - newMinimum;
                    newMinimum = this.AbsoluteMinimum;
                    newMaximum = this.AbsoluteMinimum + d;
                    if (newMaximum > this.AbsoluteMaximum)
                    {
                        newMaximum = this.AbsoluteMaximum;
                    }
                }
                else if (newMaximum > this.AbsoluteMaximum)
                {
                    double d = newMaximum - newMinimum;
                    newMaximum = this.AbsoluteMaximum;
                    newMinimum = this.AbsoluteMaximum - d;
                    if (newMinimum < this.AbsoluteMinimum)
                    {
                        newMinimum = this.AbsoluteMinimum;
                    }
                }
            }

            this.ClipMaximum = this.ViewMaximum = newMaximum;
            this.ClipMinimum = this.ViewMinimum = newMinimum;
            this.UpdateActualMaxMin();

            var deltaMinimum = this.ActualMinimum - oldMinimum;
            var deltaMaximum = this.ActualMaximum - oldMaximum;

            this.ActualMaximumAndMinimumChangedOverride();
            this.OnAxisChanged(new AxisChangedEventArgs(AxisChangeTypes.Zoom, deltaMinimum, deltaMaximum));
        }

        public virtual void Zoom(double x0, double x1)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            var oldMinimum = this.ActualMinimum;
            var oldMaximum = this.ActualMaximum;

            double newMinimum = Math.Max(Math.Min(x0, x1), this.AbsoluteMinimum);
            double newMaximum = Math.Min(Math.Max(x0, x1), this.AbsoluteMaximum);

            this.ViewMinimum = newMinimum;
            this.ViewMaximum = newMaximum;
            this.UpdateActualMaxMin();

            var deltaMinimum = this.ActualMinimum - oldMinimum;
            var deltaMaximum = this.ActualMaximum - oldMaximum;

            this.OnAxisChanged(new AxisChangedEventArgs(AxisChangeTypes.Zoom, deltaMinimum, deltaMaximum));
        }

        public virtual void ZoomAt(double factor, double x)
        {
            if (!this.IsZoomEnabled)
            {
                return;
            }

            var oldMinimum = this.ActualMinimum;
            var oldMaximum = this.ActualMaximum;

            double dx0 = (this.ActualMinimum - x) * this.scale;
            double dx1 = (this.ActualMaximum - x) * this.scale;
            this.scale *= factor;

            double newMinimum = (dx0 / this.scale) + x;
            double newMaximum = (dx1 / this.scale) + x;

            if (newMaximum - newMinimum > this.MaximumRange)
            {
                var mid = (newMinimum + newMaximum) * 0.5;
                newMaximum = mid + (this.MaximumRange * 0.5);
                newMinimum = mid - (this.MaximumRange * 0.5);
            }

            if (newMaximum - newMinimum < this.MinimumRange)
            {
                var mid = (newMinimum + newMaximum) * 0.5;
                newMaximum = mid + (this.MinimumRange * 0.5);
                newMinimum = mid - (this.MinimumRange * 0.5);
            }

            newMinimum = Math.Max(newMinimum, this.AbsoluteMinimum);
            newMaximum = Math.Min(newMaximum, this.AbsoluteMaximum);

            this.ViewMinimum = newMinimum;
            this.ViewMaximum = newMaximum;
            this.UpdateActualMaxMin();

            var deltaMinimum = this.ActualMinimum - oldMinimum;
            var deltaMaximum = this.ActualMaximum - oldMaximum;

            this.OnAxisChanged(new AxisChangedEventArgs(AxisChangeTypes.Zoom, deltaMinimum, deltaMaximum));
        }

        public virtual void ZoomAtCenter(double factor)
        {
            double sx = (this.Transform(this.ClipMaximum) + this.Transform(this.ClipMinimum)) * 0.5;
            var x = this.InverseTransform(sx);
            this.ZoomAt(factor, x);
        }
        
        public virtual void Include(double value)
        {
            if (!this.IsValidValue(value))
            {
                return;
            }

            this.DataMinimum = double.IsNaN(this.DataMinimum) ? value : Math.Min(this.DataMinimum, value);
            this.DataMaximum = double.IsNaN(this.DataMaximum) ? value : Math.Max(this.DataMaximum, value);
        }

        internal virtual void ResetDataMaxMin()
        {
            this.DataMaximum = this.DataMinimum = this.ActualMaximum = this.ActualMinimum = double.NaN;
        }

        internal virtual void UpdateActualMaxMin()
        {
            if (!double.IsNaN(this.ViewMaximum))
            {
                // The user has zoomed/panned the axis, use the ViewMaximum value.
                this.ActualMaximum = this.ViewMaximum;
            }
            else if (!double.IsNaN(this.Maximum))
            {
                // The Maximum value has been set
                this.ActualMaximum = this.Maximum;
            }
            else
            {
                // Calculate the actual maximum, including padding
                this.ActualMaximum = this.CalculateActualMaximum();
            }

            if (!double.IsNaN(this.ViewMinimum))
            {
                this.ActualMinimum = this.ViewMinimum;
            }
            else if (!double.IsNaN(this.Minimum))
            {
                this.ActualMinimum = this.Minimum;
            }
            else
            {
                this.ActualMinimum = this.CalculateActualMinimum();
            }

            this.CoerceActualMaxMin();
        }

        internal virtual void UpdateIntervals(OxyRect plotArea)
        {
            double labelSize = this.IntervalLength;
            double length = this.IsHorizontal() ? plotArea.Width : plotArea.Height;
            length *= Math.Abs(this.EndPosition - this.StartPosition);

            this.ActualMajorStep = !double.IsNaN(this.MajorStep)
                                       ? this.MajorStep
                                       : this.CalculateActualInterval(length, labelSize);

            this.ActualMinorStep = !double.IsNaN(this.MinorStep)
                                       ? this.MinorStep
                                       : this.CalculateMinorInterval(this.ActualMajorStep);

            if (double.IsNaN(this.ActualMinorStep))
            {
                this.ActualMinorStep = 2;
            }

            if (double.IsNaN(this.ActualMajorStep))
            {
                this.ActualMajorStep = 10;
            }

            this.ActualMinorStep = Math.Max(this.ActualMinorStep, this.MinimumMinorStep);
            this.ActualMajorStep = Math.Max(this.ActualMajorStep, this.MinimumMajorStep);

            this.ActualStringFormat = this.StringFormat ?? this.GetDefaultStringFormat();
        }

        internal virtual void UpdateTransform(OxyRect bounds)
        {
            double x0 = bounds.Left;
            double x1 = bounds.Right;
            double y0 = bounds.Bottom;
            double y1 = bounds.Top;

            double a0 = this.IsHorizontal() ? x0 : y0;
            double a1 = this.IsHorizontal() ? x1 : y1;

            double dx = a1 - a0;
            a1 = a0 + (this.EndPosition * dx);
            a0 = a0 + (this.StartPosition * dx);

            double marginSign = (this.IsHorizontal() ^ this.IsReversed) ? 1.0 : -1.0;

            if (this.MinimumMargin > 0)
            {
                a0 += this.MinimumMargin * marginSign;
            }

            if (this.MaximumMargin > 0)
            {
                a1 -= this.MaximumMargin * marginSign;
            }

            if (this.IsHorizontal())
            {
                this.ScreenMin = new ScreenPoint(a0, y1);
                this.ScreenMax = new ScreenPoint(a1, y0);
            }
            else if (this.IsVertical())
            {
                this.ScreenMin = new ScreenPoint(x0, a1);
                this.ScreenMax = new ScreenPoint(x1, a0);
            }

            if (this.MinimumDataMargin > 0)
            {
                a0 += this.MinimumDataMargin * marginSign;
            }

            if (this.MaximumDataMargin > 0)
            {
                a1 -= this.MaximumDataMargin * marginSign;
            }

            if (this.ActualMaximum - this.ActualMinimum < double.Epsilon)
            {
                this.ActualMaximum = this.ActualMinimum + 1;
            }

            double max = this.PreTransform(this.ActualMaximum);
            double min = this.PreTransform(this.ActualMinimum);

            double da = a0 - a1;
            double newOffset, newScale;
            if (Math.Abs(da) > double.Epsilon)
            {
                newOffset = (a0 / da * max) - (a1 / da * min);
            }
            else
            {
                newOffset = 0;
            }

            double range = max - min;
            if (Math.Abs(range) > double.Epsilon)
            {
                newScale = (a1 - a0) / range;
            }
            else
            {
                newScale = 1;
            }

            this.SetTransform(newScale, newOffset);

            if (this.MinimumDataMargin > 0)
            {
                this.ClipMinimum = this.InverseTransform(a0 - (this.MinimumDataMargin * marginSign));
            }
            else
            {
                this.ClipMinimum = this.ActualMinimum;
            }

            if (this.MaximumDataMargin > 0)
            {
                this.ClipMaximum = this.InverseTransform(a1 + (this.MaximumDataMargin * marginSign));
            }
            else
            {
                this.ClipMaximum = this.ActualMaximum;
            }

            this.ActualMaximumAndMinimumChangedOverride();
        }

        protected virtual void ActualMaximumAndMinimumChangedOverride()
        {
        }

        protected virtual string GetDefaultStringFormat()
        {
            return "g6";
        }

        protected virtual double PostInverseTransform(double x)
        {
            return x;
        }

        protected virtual double PreTransform(double x)
        {
            return x;
        }

        protected virtual double CalculateMinorInterval(double majorInterval)
        {
            return AxisUtilities.CalculateMinorInterval(majorInterval);
        }

        protected virtual IList<double> CreateTickValues(double from, double to, double step, int maxTicks = 1000)
        {
            return AxisUtilities.CreateTickValues(from, to, step, maxTicks);
        }

        protected virtual void CoerceActualMaxMin()
        {
            // Check consistency of properties
            if (this.AbsoluteMaximum <= this.AbsoluteMinimum)
            {
                throw new InvalidOperationException("AbsoluteMaximum must be larger than AbsoluteMinimum.");
            }
            if (this.AbsoluteMaximum - this.AbsoluteMinimum < this.MinimumRange)
            {
                throw new InvalidOperationException("MinimumRange must not be larger than AbsoluteMaximum-AbsoluteMinimum.");
            }
            if (this.MaximumRange < this.MinimumRange)
            {
                throw new InvalidOperationException("MinimumRange must not be larger than MaximumRange.");
            }

            // Coerce actual minimum
            if (double.IsNaN(this.ActualMinimum) || double.IsInfinity(this.ActualMinimum))
            {
                this.ActualMinimum = 0;
            }

            // Coerce actual maximum
            if (double.IsNaN(this.ActualMaximum) || double.IsInfinity(this.ActualMaximum))
            {
                this.ActualMaximum = 100;
            }

            if (this.AbsoluteMinimum > double.MinValue && this.AbsoluteMinimum < double.MaxValue)
            {
                this.ActualMinimum = Math.Max(this.ActualMinimum, this.AbsoluteMinimum);
                if (this.MaximumRange < double.MaxValue)
                {
                    this.ActualMaximum = Math.Min(this.ActualMaximum, this.AbsoluteMinimum + this.MaximumRange);
                }
            }
            if (this.AbsoluteMaximum > double.MinValue && this.AbsoluteMaximum < double.MaxValue)
            {
                this.ActualMaximum = Math.Min(this.ActualMaximum, this.AbsoluteMaximum);
                if (this.MaximumRange < double.MaxValue)
                {
                    this.ActualMinimum = Math.Max(this.ActualMinimum, this.AbsoluteMaximum - this.MaximumRange);
                }
            }

            // Coerce the minimum range
            if (this.ActualMaximum - this.ActualMinimum < this.MinimumRange)
            {
                if (this.ActualMinimum + this.MinimumRange < this.AbsoluteMaximum)
                {
                    var average = (this.ActualMaximum + this.ActualMinimum) * 0.5;
                    var delta = this.MinimumRange / 2;
                    this.ActualMinimum = average - delta;
                    this.ActualMaximum = average + delta;

                    if (this.ActualMinimum < this.AbsoluteMinimum)
                    {
                        var diff = this.AbsoluteMinimum - this.ActualMinimum;
                        this.ActualMinimum = this.AbsoluteMinimum;
                        this.ActualMaximum += diff;
                    }

                    if (this.ActualMaximum > this.AbsoluteMaximum)
                    {
                        var diff = this.AbsoluteMaximum - this.ActualMaximum;
                        this.ActualMaximum = this.AbsoluteMaximum;
                        this.ActualMinimum += diff;
                    }
                }
                else
                {
                    if (this.AbsoluteMaximum - this.MinimumRange > this.AbsoluteMinimum)
                    {
                        this.ActualMinimum = this.AbsoluteMaximum - this.MinimumRange;
                        this.ActualMaximum = this.AbsoluteMaximum;
                    }
                    else
                    {
                        this.ActualMaximum = this.AbsoluteMaximum;
                        this.ActualMinimum = this.AbsoluteMinimum;
                    }
                }
            }

            // Coerce the maximum range
            if (this.ActualMaximum - this.ActualMinimum > this.MaximumRange)
            {
                if (this.ActualMinimum + this.MaximumRange < this.AbsoluteMaximum)
                {
                    var average = (this.ActualMaximum + this.ActualMinimum) * 0.5;
                    var delta = this.MaximumRange / 2;
                    this.ActualMinimum = average - delta;
                    this.ActualMaximum = average + delta;

                    if (this.ActualMinimum < this.AbsoluteMinimum)
                    {
                        var diff = this.AbsoluteMinimum - this.ActualMinimum;
                        this.ActualMinimum = this.AbsoluteMinimum;
                        this.ActualMaximum += diff;
                    }

                    if (this.ActualMaximum > this.AbsoluteMaximum)
                    {
                        var diff = this.AbsoluteMaximum - this.ActualMaximum;
                        this.ActualMaximum = this.AbsoluteMaximum;
                        this.ActualMinimum += diff;
                    }
                }
                else
                {
                    if (this.AbsoluteMaximum - this.MaximumRange > this.AbsoluteMinimum)
                    {
                        this.ActualMinimum = this.AbsoluteMaximum - this.MaximumRange;
                        this.ActualMaximum = this.AbsoluteMaximum;
                    }
                    else
                    {
                        this.ActualMaximum = this.AbsoluteMaximum;
                        this.ActualMinimum = this.AbsoluteMinimum;
                    }
                }
            }

            // Coerce the absolute maximum/minimum
            if (this.ActualMaximum <= this.ActualMinimum)
            {
                this.ActualMaximum = this.ActualMinimum + 100;
            }
        }

        protected virtual string FormatValueOverride(double x)
        {
            // The "SuperExponentialFormat" renders the number with superscript exponents. E.g. 10^2
            if (this.UseSuperExponentialFormat && !x.Equals(0))
            {
                double exp = Exponent(x);
                double mantissa = Mantissa(x);
                string fmt;
                if (this.StringFormat == null)
                {
                    fmt = Math.Abs(mantissa - 1.0) < 1e-6 ? "10^{{{1:0}}}" : "{0}·10^{{{1:0}}}";
                }
                else
                {
                    fmt = "{0:" + this.StringFormat + "}·10^{{{1:0}}}";
                }

                return string.Format(this.ActualCulture, fmt, mantissa, exp);
            }

            string format = string.Concat("{0:", this.ActualStringFormat ?? this.StringFormat ?? string.Empty, "}");
            return string.Format(this.ActualCulture, format, x);
        }
        
        protected virtual double CalculateActualMaximum()
        {
            var actualMaximum = this.DataMaximum;
            double range = this.DataMaximum - this.DataMinimum;

            if (range < double.Epsilon)
            {
                double zeroRange = this.DataMaximum > 0 ? this.DataMaximum : 1;
                actualMaximum += zeroRange * 0.5;
            }

            if (!double.IsNaN(this.DataMinimum) && !double.IsNaN(actualMaximum))
            {
                double x1 = this.PreTransform(actualMaximum);
                double x0 = this.PreTransform(this.DataMinimum);
                double dx = this.MaximumPadding * (x1 - x0);
                return this.PostInverseTransform(x1 + dx);
            }

            return actualMaximum;
        }

        protected virtual double CalculateActualMinimum()
        {
            var actualMinimum = this.DataMinimum;
            double range = this.DataMaximum - this.DataMinimum;

            if (range < double.Epsilon)
            {
                double zeroRange = this.DataMaximum > 0 ? this.DataMaximum : 1;
                actualMinimum -= zeroRange * 0.5;
            }

            if (!double.IsNaN(this.ActualMaximum))
            {
                double x1 = this.PreTransform(this.ActualMaximum);
                double x0 = this.PreTransform(actualMinimum);
                double existingPadding = this.MaximumPadding;
                double dx = this.MinimumPadding * ((x1 - x0) / (1.0 + existingPadding));
                return this.PostInverseTransform(x0 - dx);
            }

            return actualMinimum;
        }

        protected void SetTransform(double newScale, double newOffset)
        {
            this.scale = newScale;
            this.offset = newOffset;
            this.OnTransformChanged(new EventArgs());
        }

        protected virtual double CalculateActualInterval(double availableSize, double maxIntervalSize)
        {
            return this.CalculateActualInterval(availableSize, maxIntervalSize, this.ActualMaximum - this.ActualMinimum);
        }
        
        protected double CalculateActualInterval(double availableSize, double maxIntervalSize, double range)
        {
            if (availableSize <= 0)
            {
                return maxIntervalSize;
            }

            if (Math.Abs(maxIntervalSize) < double.Epsilon)
            {
                throw new ArgumentException("Maximum interval size cannot be zero.", "maxIntervalSize");
            }

            if (Math.Abs(range) < double.Epsilon)
            {
                throw new ArgumentException("Range cannot be zero.", "range");
            }

            Func<double, double> exponent = x => Math.Ceiling(Math.Log(x, 10));
            Func<double, double> mantissa = x => x / Math.Pow(10, exponent(x) - 1);

            // reduce intervals for horizontal axis.
            // double maxIntervals = Orientation == AxisOrientation.x ? MaximumAxisIntervalsPer200Pixels * 0.8 : MaximumAxisIntervalsPer200Pixels;
            // real maximum interval count
            double maxIntervalCount = availableSize / maxIntervalSize;

            range = Math.Abs(range);
            double interval = Math.Pow(10, exponent(range));
            double intervalCandidate = interval;

            // Function to remove 'double precision noise'
            // TODO: can this be improved
            Func<double, double> removeNoise = x => double.Parse(x.ToString("e14"));

            // decrease interval until interval count becomes less than maxIntervalCount
            while (true)
            {
                var m = (int)mantissa(intervalCandidate);
                if (m == 5)
                {
                    // reduce 5 to 2
                    intervalCandidate = removeNoise(intervalCandidate / 2.5);
                }
                else if (m == 2 || m == 1 || m == 10)
                {
                    // reduce 2 to 1, 10 to 5, 1 to 0.5
                    intervalCandidate = removeNoise(intervalCandidate / 2.0);
                }
                else
                {
                    intervalCandidate = removeNoise(intervalCandidate / 2.0);
                }

                if (range / intervalCandidate > maxIntervalCount)
                {
                    break;
                }

                if (double.IsNaN(intervalCandidate) || double.IsInfinity(intervalCandidate))
                {
                    break;
                }

                interval = intervalCandidate;
            }

            return interval;
        }

        protected virtual void OnAxisChanged(AxisChangedEventArgs args)
        {
            this.UpdateActualMaxMin();

            var handler = this.AxisChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void OnTransformChanged(EventArgs args)
        {
            var handler = this.TransformChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}
