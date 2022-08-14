namespace OxyPlot
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class PdfRenderContext : ClippingRenderContext
    {
        private readonly PortableDocument doc;

        private readonly Dictionary<OxyImage, PortableDocumentImage> images
            = new Dictionary<OxyImage, PortableDocumentImage>();

        public PdfRenderContext(double width, double height, OxyColor background)
        {
            this.doc = new PortableDocument();
            this.doc.AddPage(width, height);
            this.RendersToScreen = false;

            if (background.IsVisible())
            {
                this.doc.SetFillColor(background);
                this.doc.FillRectangle(0, 0, width, height);
            }
        }

        public void Save(Stream s)
        {
            this.doc.Save(s);
        }

        public override void DrawEllipse(
            OxyRect rect,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode)
        {
            bool isStroked = stroke.IsVisible() && thickness > 0;
            bool isFilled = fill.IsVisible();
            if (!isStroked && !isFilled)
            {
                return;
            }

            double y = this.doc.PageHeight - rect.Bottom;
            if (isStroked)
            {
                this.SetLineWidth(thickness);
                this.doc.SetColor(stroke);
                if (isFilled)
                {
                    this.doc.SetFillColor(fill);
                    this.doc.DrawEllipse(rect.Left, y, rect.Width, rect.Height, true);
                }
                else
                {
                    this.doc.DrawEllipse(rect.Left, y, rect.Width, rect.Height);
                }
            }
            else
            {
                this.doc.SetFillColor(fill);
                this.doc.FillEllipse(rect.Left, y, rect.Width, rect.Height);
            }
        }

        public override void DrawLine(
            IList<ScreenPoint> points,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin)
        {
            this.doc.SetColor(stroke);
            this.SetLineWidth(thickness);
            if (dashArray != null)
            {
                this.SetLineDashPattern(dashArray, 0);
            }

            this.doc.SetLineJoin(Convert(lineJoin));
            double h = this.doc.PageHeight;
            this.doc.MoveTo(points[0].X, h - points[0].Y);
            for (int i = 1; i < points.Count; i++)
            {
                this.doc.LineTo(points[i].X, h - points[i].Y);
            }

            this.doc.Stroke(false);
            if (dashArray != null)
            {
                this.doc.ResetLineDashPattern();
            }
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
            bool isStroked = stroke.IsVisible() && thickness > 0;
            bool isFilled = fill.IsVisible();
            if (!isStroked && !isFilled)
            {
                return;
            }

            double h = this.doc.PageHeight;
            this.doc.MoveTo(points[0].X, h - points[0].Y);
            for (int i = 1; i < points.Count; i++)
            {
                this.doc.LineTo(points[i].X, h - points[i].Y);
            }

            if (isStroked)
            {
                this.doc.SetColor(stroke);
                this.SetLineWidth(thickness);
                if (dashArray != null)
                {
                    this.SetLineDashPattern(dashArray, 0);
                }

                this.doc.SetLineJoin(Convert(lineJoin));
                if (isFilled)
                {
                    this.doc.SetFillColor(fill);
                    this.doc.FillAndStroke();
                }
                else
                {
                    this.doc.Stroke();
                }

                if (dashArray != null)
                {
                    this.doc.ResetLineDashPattern();
                }
            }
            else
            {
                this.doc.SetFillColor(fill);
                this.doc.Fill();
            }
        }


        public override void DrawRectangle(
            OxyRect rect,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode)
        {
            bool isStroked = stroke.IsVisible() && thickness > 0;
            bool isFilled = fill.IsVisible();
            if (!isStroked && !isFilled)
            {
                return;
            }

            double y = this.doc.PageHeight - rect.Bottom;
            if (isStroked)
            {
                this.SetLineWidth(thickness);
                this.doc.SetColor(stroke);
                if (isFilled)
                {
                    this.doc.SetFillColor(fill);
                    this.doc.DrawRectangle(rect.Left, y, rect.Width, rect.Height, true);
                }
                else
                {
                    this.doc.DrawRectangle(rect.Left, y, rect.Width, rect.Height);
                }
            }
            else
            {
                this.doc.SetFillColor(fill);
                this.doc.FillRectangle(rect.Left, y, rect.Width, rect.Height);
            }
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
            this.doc.SaveState();
            this.doc.SetFont(fontFamily, fontSize / 96 * 72, fontWeight > 500);
            this.doc.SetFillColor(fill);

            double width, height;
            this.doc.MeasureText(text, out width, out height);
            if (maxSize != null)
            {
                if (width > maxSize.Value.Width)
                {
                    width = Math.Max(maxSize.Value.Width, 0);
                }

                if (height > maxSize.Value.Height)
                {
                    height = Math.Max(maxSize.Value.Height, 0);
                }
            }

            double dx = 0;
            if (halign == HorizontalAlignment.Center)
            {
                dx = -width / 2;
            }

            if (halign == HorizontalAlignment.Right)
            {
                dx = -width;
            }

            double dy = 0;

            if (valign == VerticalAlignment.Middle)
            {
                dy = -height / 2;
            }

            if (valign == VerticalAlignment.Top)
            {
                dy = -height;
            }

            double y = this.doc.PageHeight - p.Y;

            this.doc.Translate(p.X, y);
            if (Math.Abs(rotate) > 1e-6)
            {
                this.doc.Rotate(-rotate);
            }

            this.doc.Translate(dx, dy);

            this.doc.SetClippingRectangle(0, 0, width, height);
            this.doc.DrawText(0, 0, text);
            this.doc.RestoreState();
        }

        public override OxySize MeasureText(string text, string fontFamily, double fontSize, double fontWeight)
        {
            this.doc.SetFont(fontFamily, fontSize / 96 * 72, fontWeight > 500);
            double width, height;
            this.doc.MeasureText(text, out width, out height);
            return new OxySize(width, height);
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
            PortableDocumentImage image;
            if (!this.images.TryGetValue(source, out image))
            {
                image = PortableDocumentImageUtilities.Convert(source, interpolate);
                if (image == null)
                {
                    return;
                }

                this.images[source] = image;
            }

            this.doc.SaveState();
            double x = destX - (srcX / srcWidth * destWidth);
            double width = image.Width / srcWidth * destWidth;
            double y = destY - (srcY / srcHeight * destHeight);
            double height = image.Height / srcHeight * destHeight;
            this.doc.SetClippingRectangle(destX, this.doc.PageHeight - (destY - destHeight), destWidth, destHeight);
            this.doc.Translate(x, this.doc.PageHeight - (y + height));
            this.doc.Scale(width, height);
            this.doc.DrawImage(image);
            this.doc.RestoreState();
        }

        protected override void SetClip(OxyRect clippingRectangle)
        {
            this.doc.SaveState();
            this.doc.SetClippingRectangle(clippingRectangle.Left, clippingRectangle.Bottom, clippingRectangle.Width, clippingRectangle.Height);
        }

        protected override void ResetClip()
        {
            this.doc.RestoreState();
        }

        private static LineJoin Convert(LineJoin lineJoin)
        {
            switch (lineJoin)
            {
                case LineJoin.Bevel:
                    return LineJoin.Bevel;
                case LineJoin.Miter:
                    return LineJoin.Miter;
                default:
                    return LineJoin.Round;
            }
        }

        private void SetLineWidth(double thickness)
        {
            this.doc.SetLineWidth(thickness / 96 * 72);
        }


        private void SetLineDashPattern(double[] dashArray, double dashPhase)
        {
            this.doc.SetLineDashPattern(
                dashArray.Select(d => d / 96 * 72).ToArray(),
                dashPhase / 96 * 72);
        }
    }
}
