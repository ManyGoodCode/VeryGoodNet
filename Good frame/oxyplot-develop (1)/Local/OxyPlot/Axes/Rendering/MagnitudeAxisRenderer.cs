
namespace OxyPlot.Axes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MagnitudeAxisRenderer : AxisRendererBase
    {
        public MagnitudeAxisRenderer(IRenderContext rc, PlotModel plot)
            : base(rc, plot)
        {
        }


        public override void Render(Axis axis, int pass)
        {
            base.Render(axis, pass);

            var angleAxis = this.Plot.DefaultAngleAxis;

            if (angleAxis == null)
            {
                throw new NullReferenceException("Angle axis should not be null.");
            }

            angleAxis.UpdateActualMaxMin();

            if (pass == 0 && this.ExtraPen != null)
            {
                var extraTicks = axis.ExtraGridlines;
                if (extraTicks != null)
                {
                    for (int i = 0; i < extraTicks.Length; i++)
                    {
                        this.RenderTick(axis, angleAxis, extraTicks[i], this.ExtraPen);
                    }
                }
            }

            if (pass == 0 && this.MinorPen != null)
            {
                foreach (var tickValue in this.MinorTickValues)
                {
                    this.RenderTick(axis, angleAxis, tickValue, this.MinorPen);
                }
            }

            if (pass == 0 && this.MajorPen != null)
            {
                foreach (var tickValue in this.MajorTickValues)
                {
                    this.RenderTick(axis, angleAxis, tickValue, this.MajorPen);
                }
            }

            if (pass == 1)
            {
                foreach (var tickValue in this.MajorTickValues)
                {
                    this.RenderTickText(axis, tickValue, angleAxis);
                }
            }
        }

        private static double GetActualAngle(Axis axis, Axis angleAxis)
        {
            var a = axis.Transform(0, angleAxis.Angle, angleAxis);
            var b = axis.Transform(1, angleAxis.Angle, angleAxis);
            return Math.Atan2(b.y - a.y, b.x - a.x);
        }

        private static void GetTickTextAligment(double actualAngle, out HorizontalAlignment ha, out VerticalAlignment va)
        {
            if (actualAngle > 3 * Math.PI / 4 || actualAngle < -3 * Math.PI / 4)
            {
                ha = HorizontalAlignment.Center;
                va = VerticalAlignment.Top;
            }
            else if (actualAngle < -Math.PI / 4)
            {
                ha = HorizontalAlignment.Right;
                va = VerticalAlignment.Middle;
            }
            else if (actualAngle > Math.PI / 4)
            {
                ha = HorizontalAlignment.Left;
                va = VerticalAlignment.Middle;
            }
            else
            {
                ha = HorizontalAlignment.Center;
                va = VerticalAlignment.Bottom;
            }
        }

        private void RenderTick(Axis axis, AngleAxis angleAxis, double x, OxyPen pen)
        {
            var isFullCircle = Math.Abs(Math.Abs(angleAxis.EndAngle - angleAxis.StartAngle) - 360) < 1e-6;

            if (isFullCircle && pen.ActualDashArray == null)
            {
                this.RenderTickCircle(axis, angleAxis, x, pen);
            }
            else
            {
                this.RenderTickArc(axis, angleAxis, x, pen);
            }
        }
        
        private void RenderTickCircle(Axis axis, Axis angleAxis, double x, OxyPen pen)
        {
            var zero = angleAxis.Offset;
            var center = axis.Transform(axis.ClipMinimum, zero, angleAxis);
            var right = axis.Transform(x, zero, angleAxis).X;
            var radius = right - center.X;
            var width = radius * 2;
            var left = right - width;
            var top = center.Y - radius;
            var height = width;

            this.RenderContext.DrawEllipse(new OxyRect(left, top, width, height), OxyColors.Undefined, pen.Color, pen.Thickness, axis.EdgeRenderingMode);
        }


        private void RenderTickArc(Axis axis, AngleAxis angleAxis, double x, OxyPen pen)
        {
            // caution: make sure angleAxis.UpdateActualMaxMin(); has been called
            var minAngle = angleAxis.ClipMinimum;
            var maxAngle = angleAxis.ClipMaximum;

            const double MaxSegments = 90.0;
            var segmentCount = (int)(MaxSegments * Math.Abs(angleAxis.EndAngle - angleAxis.StartAngle) / 360.0);

            var angleStep = (maxAngle - minAngle) / (segmentCount - 1);

            var points = new List<ScreenPoint>();

            for (var i = 0; i < segmentCount; i++)
            {
                var angle = minAngle + (i * angleStep);
                points.Add(axis.Transform(x, angle, angleAxis));
            }

            this.RenderContext.DrawLine(points, pen.Color, pen.Thickness, axis.EdgeRenderingMode, pen.ActualDashArray);
        }

        private void RenderTickText(Axis axis, double x, Axis angleAxis)
        {
            var actualAngle = GetActualAngle(axis, angleAxis);
            var dx = axis.AxisTickToLabelDistance * Math.Sin(actualAngle);
            var dy = -axis.AxisTickToLabelDistance * Math.Cos(actualAngle);

            HorizontalAlignment ha;
            VerticalAlignment va;
            GetTickTextAligment(actualAngle, out ha, out va);

            var pt = axis.Transform(x, angleAxis.Angle, angleAxis);
            pt = new ScreenPoint(pt.X + dx, pt.Y + dy);

            string text = axis.FormatValue(x);
            this.RenderContext.DrawMathText(
                pt,
                text,
                axis.ActualTextColor,
                axis.ActualFont,
                axis.ActualFontSize,
                axis.ActualFontWeight,
                axis.Angle,
                ha,
                va);
        }
    }
}
