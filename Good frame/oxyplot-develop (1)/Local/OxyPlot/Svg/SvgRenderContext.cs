namespace OxyPlot
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class SvgRenderContext : ClippingRenderContext, IDisposable
    {
        private readonly SvgWriter w;
        private bool disposed;

        public SvgRenderContext(Stream s, double width, double height, bool isDocument, IRenderContext textMeasurer, OxyColor background, bool useVerticalTextAlignmentWorkaround = false)
        {
            if (textMeasurer == null)
            {
                throw new ArgumentNullException("textMeasurer", "A text measuring render context must be provided.");
            }

            this.w = new SvgWriter(s, width, height, isDocument);
            this.TextMeasurer = textMeasurer;
            this.UseVerticalTextAlignmentWorkaround = useVerticalTextAlignmentWorkaround;

            if (background.IsVisible())
            {
                this.w.WriteRectangle(0, 0, width, height, this.w.CreateStyle(background, OxyColors.Undefined, 0), EdgeRenderingMode.Adaptive);
            }
        }

        public IRenderContext TextMeasurer { get; set; }
        public bool UseVerticalTextAlignmentWorkaround { get; set; }
        public void Close()
        {
            this.w.Close();
        }

        public void Complete()
        {
            this.w.Complete();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override void DrawEllipse(OxyRect rect, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
            this.w.WriteEllipse(rect.Left, rect.Top, rect.Width, rect.Height, this.w.CreateStyle(fill, stroke, thickness), edgeRenderingMode);
        }

        public override void DrawLine(
            IList<ScreenPoint> points,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin)
        {
            this.w.WritePolyline(points, this.w.CreateStyle(OxyColors.Undefined, stroke, thickness, dashArray, lineJoin), edgeRenderingMode);
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
            this.w.WritePolygon(points, this.w.CreateStyle(fill, stroke, thickness, dashArray, lineJoin), edgeRenderingMode);
        }

        public override void DrawRectangle(OxyRect rect, OxyColor fill, OxyColor stroke, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
            this.w.WriteRectangle(rect.Left, rect.Top, rect.Width, rect.Height, this.w.CreateStyle(fill, stroke, thickness), edgeRenderingMode);
        }

        public override void DrawText(
            ScreenPoint p,
            string text,
            OxyColor c,
            string fontFamily,
            double fontSize,
            double fontWeight,
            double rotate,
            HorizontalAlignment halign,
            VerticalAlignment valign,
            OxySize? maxSize)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var lines = StringHelper.SplitLines(text);

            var textSize = this.MeasureText(text, fontFamily, fontSize, fontWeight);
            var lineHeight = textSize.Height / lines.Length;
            var lineOffset = new ScreenVector(-Math.Sin(rotate / 180.0 * Math.PI) * lineHeight, +Math.Cos(rotate / 180.0 * Math.PI) * lineHeight);
            
            if (this.UseVerticalTextAlignmentWorkaround)
            {
                // offset the position, and set the valign to neutral value of `Bottom`
                double offsetRatio = valign == VerticalAlignment.Bottom ? (1.0 - lines.Length) : valign == VerticalAlignment.Top ? 1.0 : (1.0 - (lines.Length / 2.0));
                valign = VerticalAlignment.Bottom;

                p += lineOffset * offsetRatio;

                foreach (var line in lines)
                {
                    var size = this.MeasureText(line, fontFamily, fontSize, fontWeight);
                    this.w.WriteText(p, line, c, fontFamily, fontSize, fontWeight, rotate, halign, valign);

                    p += lineOffset;
                }
            }
            else
            {
                if (valign == VerticalAlignment.Bottom)
                {
                    for (var i = lines.Length - 1; i >= 0; i--)
                    {
                        var line = lines[i];
                        var size = this.MeasureText(line, fontFamily, fontSize, fontWeight);
                        this.w.WriteText(p, line, c, fontFamily, fontSize, fontWeight, rotate, halign, valign);

                        p -= lineOffset;
                    }
                }
                else
                {
                    foreach (var line in lines)
                    {
                        var size = this.MeasureText(line, fontFamily, fontSize, fontWeight);
                        this.w.WriteText(p, line, c, fontFamily, fontSize, fontWeight, rotate, halign, valign);

                        p += lineOffset;
                    }
                }
            }
        }

        public void Flush()
        {
            this.w.Flush();
        }

        public override OxySize MeasureText(string text, string fontFamily, double fontSize, double fontWeight)
        {
            if (string.IsNullOrEmpty(text))
            {
                return OxySize.Empty;
            }

            return this.TextMeasurer.MeasureText(text, fontFamily, fontSize, fontWeight);
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
            this.w.WriteImage(srcX, srcY, srcWidth, srcHeight, destX, destY, destWidth, destHeight, source);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.w.Dispose();
                }
            }

            this.disposed = true;
        }

        protected override void SetClip(OxyRect clippingRectangle)
        {
            this.w.BeginClip(clippingRectangle.Left, clippingRectangle.Top, clippingRectangle.Width, clippingRectangle.Height);
        }

        protected override void ResetClip()
        {
            this.w.EndClip();
        }
    }
}
