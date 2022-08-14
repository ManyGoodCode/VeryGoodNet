namespace OxyPlot
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public class SvgWriter : XmlWriterBase
    {
        private bool endIsWritten;
        private int clipPathNumber = 1;

        public SvgWriter(Stream stream, double width, double height, bool isDocument = true)
            : base(stream)
        {
            this.IsDocument = isDocument;
            this.NumberFormat = "0.####";
            this.WriteHeader(width, height);
        }

        public bool IsDocument { get; set; }

        public string NumberFormat { get; set; }
        public override void Close()
        {
            if (!this.endIsWritten)
            {
                this.Complete();
            }

            base.Close();
        }

        public void Complete()
        {
            this.WriteEndElement();
            if (this.IsDocument)
            {
                this.WriteEndDocument();
            }

            this.endIsWritten = true;
        }

        public string CreateStyle(
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            double[] dashArray = null,
            LineJoin lineJoin = LineJoin.Miter)
        {
            var style = new StringBuilder();
            if (fill.IsInvisible())
            {
                style.AppendFormat("fill:none;");
            }
            else
            {
                style.AppendFormat("fill:{0};", this.ColorToString(fill));
                if (fill.A != 0xFF)
                {
                    style.AppendFormat(CultureInfo.InvariantCulture, "fill-opacity:{0};", fill.A / 255.0);
                }
            }

            if (stroke.IsInvisible())
            {
                style.AppendFormat("stroke:none;");
            }
            else
            {
                string formatString = "stroke:{0};stroke-width:{1:" + this.NumberFormat + "}";
                style.AppendFormat(CultureInfo.InvariantCulture, formatString, this.ColorToString(stroke), thickness);
                switch (lineJoin)
                {
                    case LineJoin.Round:
                        style.AppendFormat(";stroke-linejoin:round");
                        break;
                    case LineJoin.Bevel:
                        style.AppendFormat(";stroke-linejoin:bevel");
                        break;
                }

                if (stroke.A != 0xFF)
                {
                    style.AppendFormat(CultureInfo.InvariantCulture, ";stroke-opacity:{0}", stroke.A / 255.0);
                }

                if (dashArray != null && dashArray.Length > 0)
                {
                    style.Append(";stroke-dasharray:");
                    for (int i = 0; i < dashArray.Length; i++)
                    {
                        style.AppendFormat(
                            CultureInfo.InvariantCulture, "{0}{1}", i > 0 ? "," : string.Empty, dashArray[i]);
                    }
                }
            }

            return style.ToString();
        }

        public void WriteEllipse(double x, double y, double width, double height, string style, EdgeRenderingMode edgeRenderingMode)
        {
            this.WriteStartElement("ellipse");
            this.WriteAttributeString("cx", x + (width / 2));
            this.WriteAttributeString("cy", y + (height / 2));
            this.WriteAttributeString("rx", width / 2);
            this.WriteAttributeString("ry", height / 2);
            this.WriteAttributeString("style", style);
            this.WriteEdgeRenderingModeAttribute(edgeRenderingMode);
            this.WriteEndElement();
        }


        public void BeginClip(double x, double y, double width, double height)
        {
            var clipPath = "clipPath" + this.clipPathNumber++;

            this.WriteStartElement("defs");
            this.WriteStartElement("clipPath");
            this.WriteAttributeString("id", clipPath);
            this.WriteStartElement("rect");
            this.WriteAttributeString("x", x);
            this.WriteAttributeString("y", y);
            this.WriteAttributeString("width", width);
            this.WriteAttributeString("height", height);
            this.WriteEndElement(); // rect
            this.WriteEndElement(); // clipPath
            this.WriteEndElement(); // defs

            this.WriteStartElement("g");
            this.WriteAttributeString("clip-path", $"url(#{clipPath})");
        }

        public void EndClip()
        {
            this.WriteEndElement(); // g
        }

        public void WriteImage(
            double srcX,
            double srcY,
            double srcWidth,
            double srcHeight,
            double destX,
            double destY,
            double destWidth,
            double destHeight,
            OxyImage image)
        {
            double x = destX - (srcX / srcWidth * destWidth);
            double width = image.Width / srcWidth * destWidth;
            double y = destY - (srcY / srcHeight * destHeight);
            double height = image.Height / srcHeight * destHeight;
            this.BeginClip(destX, destY, destWidth, destHeight);
            this.WriteImage(x, y, width, height, image);
            this.EndClip();
        }

        public void WriteImage(double x, double y, double width, double height, OxyImage image)
        {
            this.WriteStartElement("image");
            this.WriteAttributeString("x", x);
            this.WriteAttributeString("y", y);
            this.WriteAttributeString("width", width);
            this.WriteAttributeString("height", height);
            this.WriteAttributeString("preserveAspectRatio", "none");
            var imageData = image.GetData();
            var encodedImage = new StringBuilder();
            encodedImage.Append("data:");
            encodedImage.Append("image/png");
            encodedImage.Append(";base64,");
            encodedImage.Append(Convert.ToBase64String(imageData));
            this.WriteAttributeString("xlink", "href", null, encodedImage.ToString());
            this.WriteEndElement();
        }

        public void WriteLine(ScreenPoint p1, ScreenPoint p2, string style, EdgeRenderingMode edgeRenderingMode)
        {
            this.WriteStartElement("line");
            this.WriteAttributeString("x1", p1.X);
            this.WriteAttributeString("y1", p1.Y);
            this.WriteAttributeString("x2", p2.X);
            this.WriteAttributeString("y2", p2.Y);
            this.WriteAttributeString("style", style);
            this.WriteEdgeRenderingModeAttribute(edgeRenderingMode);
            this.WriteEndElement();
        }

        public void WritePolygon(IEnumerable<ScreenPoint> points, string style, EdgeRenderingMode edgeRenderingMode)
        {
            this.WriteStartElement("polygon");
            this.WriteAttributeString("points", this.PointsToString(points));
            this.WriteAttributeString("style", style);
            this.WriteEdgeRenderingModeAttribute(edgeRenderingMode);
            this.WriteEndElement();
        }


        public void WritePolyline(IEnumerable<ScreenPoint> pts, string style, EdgeRenderingMode edgeRenderingMode)
        {
            this.WriteStartElement("polyline");
            this.WriteAttributeString("points", this.PointsToString(pts));
            this.WriteAttributeString("style", style);
            this.WriteEdgeRenderingModeAttribute(edgeRenderingMode);
            this.WriteEndElement();
        }


        public void WriteRectangle(double x, double y, double width, double height, string style, EdgeRenderingMode edgeRenderingMode)
        {
            this.WriteStartElement("rect");
            this.WriteAttributeString("x", x);
            this.WriteAttributeString("y", y);
            this.WriteAttributeString("width", width);
            this.WriteAttributeString("height", height);
            this.WriteAttributeString("style", style);
            this.WriteEdgeRenderingModeAttribute(edgeRenderingMode);
            this.WriteEndElement();
        }


        public void WriteText(
            ScreenPoint position,
            string text,
            OxyColor fill,
            string fontFamily = null,
            double fontSize = 10,
            double fontWeight = FontWeights.Normal,
            double rotate = 0,
            HorizontalAlignment halign = HorizontalAlignment.Left,
            VerticalAlignment valign = VerticalAlignment.Top)
        {
            this.WriteStartElement("text");

            string baselineAlignment = "hanging";
            if (valign == VerticalAlignment.Middle)
            {
                baselineAlignment = "middle";
            }

            if (valign == VerticalAlignment.Bottom)
            {
                baselineAlignment = "baseline";
            }

            this.WriteAttributeString("dominant-baseline", baselineAlignment);

            string textAnchor = "start";
            if (halign == HorizontalAlignment.Center)
            {
                textAnchor = "middle";
            }

            if (halign == HorizontalAlignment.Right)
            {
                textAnchor = "end";
            }

            this.WriteAttributeString("text-anchor", textAnchor);

            string fmt = "translate({0:" + this.NumberFormat + "},{1:" + this.NumberFormat + "})";
            string transform = string.Format(CultureInfo.InvariantCulture, fmt, position.X, position.Y);
            if (Math.Abs(rotate) > 0)
            {
                transform += string.Format(CultureInfo.InvariantCulture, " rotate({0})", rotate);
            }

            this.WriteAttributeString("transform", transform);

            if (fontFamily != null)
            {
                this.WriteAttributeString("font-family", fontFamily);
            }

            if (fontSize > 0)
            {
                this.WriteAttributeString("font-size", fontSize);
            }

            if (fontWeight > 0)
            {
                this.WriteAttributeString("font-weight", fontWeight);
            }

            if (fill.IsInvisible())
            {
                this.WriteAttributeString("fill", "none");
            }
            else
            {
                this.WriteAttributeString("fill", this.ColorToString(fill));
                if (fill.A != 0xFF)
                {
                    this.WriteAttributeString("fill-opacity", fill.A / 255.0);
                }
            }

            this.WriteString(text);
            this.WriteEndElement();
        }

        protected string ColorToString(OxyColor color)
        {
            if (color.Equals(OxyColors.Black))
            {
                return "black";
            }

            var formatString = "rgb({0:" + this.NumberFormat + "},{1:" + this.NumberFormat + "},{2:" + this.NumberFormat + "})";
            return string.Format(formatString, color.R, color.G, color.B);
        }

        protected void WriteAttributeString(string name, double value)
        {
            this.WriteAttributeString(name, value.ToString(this.NumberFormat, CultureInfo.InvariantCulture));
        }


        private void WriteEdgeRenderingModeAttribute(EdgeRenderingMode edgeRenderingMode)
        {
            string value;
            switch (edgeRenderingMode)
            {
                case EdgeRenderingMode.PreferSharpness:
                    value = "crispEdges";
                    break;
                case EdgeRenderingMode.PreferSpeed:
                    value = "optimizeSpeed";
                    break;
                case EdgeRenderingMode.PreferGeometricAccuracy:
                    value = "geometricPrecision";
                    break;
                default:
                    return;
            }

            this.WriteAttributeString("shape-rendering", value);
        }

        private string GetAutoValue(double value, string auto)
        {
            if (double.IsNaN(value))
            {
                return auto;
            }

            return value.ToString(this.NumberFormat, CultureInfo.InvariantCulture);
        }

        private string PointsToString(IEnumerable<ScreenPoint> points)
        {
            var sb = new StringBuilder();
            string fmt = "{0:" + this.NumberFormat + "},{1:" + this.NumberFormat + "} ";
            foreach (var p in points)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, fmt, p.X, p.Y);
            }

            return sb.ToString().Trim();
        }

        private void WriteHeader(double width, double height)
        {
            if (this.IsDocument)
            {
                this.WriteStartDocument(false);
                this.WriteDocType(
                    "svg", "-//W3C//DTD SVG 1.1//EN", "http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd", null);
            }

            this.WriteStartElement("svg", "http://www.w3.org/2000/svg");
            this.WriteAttributeString("width", this.GetAutoValue(width, "100%"));
            this.WriteAttributeString("height", this.GetAutoValue(height, "100%"));
            this.WriteAttributeString("version", "1.1");
            this.WriteAttributeString("xmlns", "xlink", null, "http://www.w3.org/1999/xlink");
        }
    }
}
