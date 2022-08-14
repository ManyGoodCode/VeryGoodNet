namespace OxyPlot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class XkcdRenderingDecorator : RenderContextBase
    {
        private readonly IRenderContext rc;
        private readonly Random r = new Random(0);

        public XkcdRenderingDecorator(IRenderContext rc)
        {
            this.rc = rc;
            this.RendersToScreen = this.rc.RendersToScreen;

            this.DistortionFactor = 7;
            this.InterpolationDistance = 10;
            this.ThicknessScale = 2;

            this.FontFamily = "Humor Sans"; // http://antiyawn.com/uploads/humorsans.html
        }

        public double DistortionFactor { get; set; }
        public double InterpolationDistance { get; set; }
        public string FontFamily { get; set; }
        public double ThicknessScale { get; set; }

        public override int ClipCount => this.rc.ClipCount;

        public override void DrawLine(
            IList<ScreenPoint> points,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin)
        {
            ScreenPoint[] xckdPoints = this.Distort(points);
            this.rc.DrawLine(xckdPoints, stroke, thickness * this.ThicknessScale, edgeRenderingMode, dashArray, lineJoin);
        }

        public override void DrawPolygon(
            IList<ScreenPoint> points,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin)
        {
            List<ScreenPoint> p = new List<ScreenPoint>(points);
            p.Add(p[0]);

            ScreenPoint[] xckdPoints = this.Distort(p);
            this.rc.DrawPolygon(xckdPoints, fill, stroke, thickness * this.ThicknessScale, edgeRenderingMode, dashArray, lineJoin);
        }

        public override void DrawText(
            ScreenPoint p,
            string text,
            OxyColor fill,
            string fontFamily,
            double fontSize,
            double fontWeight,
            double rotate,
            HorizontalAlignment halign,
            VerticalAlignment valign,
            OxySize? maxSize)
        {
            this.rc.DrawText(p, text, fill, this.GetFontFamily(fontFamily), fontSize, fontWeight, rotate, halign, valign, maxSize);
        }

        public override OxySize MeasureText(string text, string fontFamily, double fontSize, double fontWeight)
        {
            return this.rc.MeasureText(text, this.GetFontFamily(fontFamily), fontSize, fontWeight);
        }

        public override void SetToolTip(string text)
        {
            this.rc.SetToolTip(text);
        }

        public override void CleanUp()
        {
            this.rc.CleanUp();
        }

        public override void DrawImage(
            OxyImage source,
            double srcX,
            double srcY,
            double srcWidth,
            double srcHeight,
            double destX,
            double destY,
            double destWidth,
            double destHeight,
            double opacity,
            bool interpolate)
        {
            this.rc.DrawImage(source, srcX, srcY, srcWidth, srcHeight, destX, destY, destWidth, destHeight, opacity, interpolate);
        }

        public override void PushClip(OxyRect clippingRectangle)
        {
            this.rc.PushClip(clippingRectangle);
        }

        public override void PopClip()
        {
            this.rc.PopClip();
        }

        private string GetFontFamily(string fontFamily)
        {
            return this.FontFamily;
        }

        private ScreenPoint[] Distort(IEnumerable<ScreenPoint> points)
        {
            IList<ScreenPoint> interpolated = this.Interpolate(points, this.InterpolationDistance).ToArray();
            ScreenPoint[] result = new ScreenPoint[interpolated.Count];
            double[] randomNumbers = this.GenerateRandomNumbers(interpolated.Count);
            randomNumbers = this.ApplyMovingAverage(randomNumbers, 5);

            double d = this.DistortionFactor;
            double d2 = d / 2;
            for (int i = 0; i < interpolated.Count; i++)
            {
                if (i == 0 || i == interpolated.Count - 1)
                {
                    result[i] = interpolated[i];
                    continue;
                }

                var tangent = interpolated[i + 1] - interpolated[i - 1];
                tangent.Normalize();
                var normal = new ScreenVector(tangent.Y, -tangent.X);

                var delta = normal * ((randomNumbers[i] * d) - d2);
                result[i] = interpolated[i] + delta;
            }

            return result;
        }

        private double[] GenerateRandomNumbers(int n)
        {
            var result = new double[n];
            for (int i = 0; i < n; i++)
            {
                result[i] = this.r.NextDouble();
            }

            return result;
        }

        private double[] ApplyMovingAverage(IList<double> input, int m)
        {
            int n = input.Count;
            var result = new double[n];
            var m2 = m / 2;
            for (int i = 0; i < n; i++)
            {
                var j0 = Math.Max(0, i - m2);
                var j1 = Math.Min(n - 1, i + m2);
                for (int j = j0; j <= j1; j++)
                {
                    result[i] += input[j];
                }

                result[i] /= m;
            }

            return result;
        }

        private IEnumerable<ScreenPoint> Interpolate(IEnumerable<ScreenPoint> input, double dist)
        {
            var p0 = default(ScreenPoint);
            double l = -1;
            double nl = dist;
            foreach (var p1 in input)
            {
                if (l < 0)
                {
                    yield return p1;
                    p0 = p1;
                    l = 0;
                    continue;
                }

                var dp = p1 - p0;
                var l1 = dp.Length;

                if (l1 > 0)
                {
                    while (nl >= l && nl <= l + l1)
                    {
                        var f = (nl - l) / l1;
                        yield return new ScreenPoint((p0.X * (1 - f)) + (p1.X * f), (p0.Y * (1 - f)) + (p1.Y * f));

                        nl += dist;
                    }
                }

                l += l1;
                p0 = p1;
            }

            yield return p0;
        }
    }
}
