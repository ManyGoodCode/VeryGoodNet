
namespace OxyPlot.Axes
{
    using System.Collections.Generic;

    public abstract class AxisRendererBase
    {
        private readonly PlotModel plot;
        private readonly IRenderContext rc;
        private IList<double> majorLabelValues;
        private IList<double> majorTickValues;
        private IList<double> minorTickValues;
        protected AxisRendererBase(IRenderContext rc, PlotModel plot)
        {
            this.plot = plot;
            this.rc = rc;
        }
        
        protected PlotModel Plot
        {
            get
            {
                return this.plot;
            }
        }

        protected IRenderContext RenderContext
        {
            get
            {
                return this.rc;
            }
        }

        protected OxyPen AxislinePen { get; set; }
        protected OxyPen ExtraPen { get; set; }
        protected IList<double> MajorLabelValues
        {
            get
            {
                return this.majorLabelValues;
            }

            set
            {
                this.majorLabelValues = value;
            }
        }
        
        protected OxyPen MajorPen { get; set; }
        protected OxyPen MajorTickPen { get; set; }
        protected IList<double> MajorTickValues
        {
            get
            {
                return this.majorTickValues;
            }

            set
            {
                this.majorTickValues = value;
            }
        }

        protected OxyPen MinorPen { get; set; }
        protected OxyPen MinorTickPen { get; set; }
        protected IList<double> MinorTickValues
        {
            get
            {
                return this.minorTickValues;
            }

            set
            {
                this.minorTickValues = value;
            }
        }

        protected OxyPen ZeroPen { get; set; }
        public virtual void Render(Axis axis, int pass)
        {
            if (axis == null)
            {
                return;
            }

            axis.GetTickValues(out this.majorLabelValues, out this.majorTickValues, out this.minorTickValues);
            this.CreatePens(axis);
        }

        protected virtual void CreatePens(Axis axis)
        {
            var minorTickColor = axis.MinorTicklineColor.IsAutomatic() ? axis.TicklineColor : axis.MinorTicklineColor;

            this.MinorPen = OxyPen.Create(axis.MinorGridlineColor, axis.MinorGridlineThickness, axis.MinorGridlineStyle);
            this.MajorPen = OxyPen.Create(axis.MajorGridlineColor, axis.MajorGridlineThickness, axis.MajorGridlineStyle);
            this.MinorTickPen = OxyPen.Create(minorTickColor, axis.MinorGridlineThickness);
            this.MajorTickPen = OxyPen.Create(axis.TicklineColor, axis.MajorGridlineThickness);
            this.ZeroPen = OxyPen.Create(axis.TicklineColor, axis.MajorGridlineThickness);
            this.ExtraPen = OxyPen.Create(axis.ExtraGridlineColor, axis.ExtraGridlineThickness, axis.ExtraGridlineStyle);
            this.AxislinePen = OxyPen.Create(axis.AxislineColor, axis.AxislineThickness, axis.AxislineStyle);
        }

        protected virtual void GetTickPositions(Axis axis, TickStyle tickStyle, double tickSize, AxisPosition position, out double x0, out double x1)
        {
            x0 = 0;
            x1 = 0;
            bool isTopOrLeft = position == AxisPosition.Top || position == AxisPosition.Left;
            double sign = isTopOrLeft ? -1 : 1;
            switch (tickStyle)
            {
                case TickStyle.Crossing:
                    x0 = -tickSize * sign * 0.75;
                    x1 = tickSize * sign * 0.75;
                    break;
                case TickStyle.Inside:
                    x0 = -tickSize * sign;
                    break;
                case TickStyle.Outside:
                    x1 = tickSize * sign;
                    break;
            }
        }

        protected bool IsWithin(double d, double min, double max)
        {
            if (d < min)
            {
                return false;
            }

            if (d > max)
            {
                return false;
            }

            return true;
        }
    }
}
