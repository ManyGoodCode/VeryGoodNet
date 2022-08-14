
namespace OxyPlot
{
    using System.IO;

    public class SvgExporter : IExporter
    {
        public SvgExporter()
        {
            this.Width = 600;
            this.Height = 400;
            this.IsDocument = true;
        }

        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsDocument { get; set; }
        public bool UseVerticalTextAlignmentWorkaround { get; set; }
        public IRenderContext TextMeasurer { get; set; }

        public static void Export(IPlotModel model, Stream stream, double width, double height, bool isDocument, IRenderContext textMeasurer = null, bool useVerticalTextAlignmentWorkaround = false)
        {
            if (textMeasurer == null)
            {
                textMeasurer = new PdfRenderContext(width, height, model.Background);
            }

            using (var rc = new SvgRenderContext(stream, width, height, isDocument, textMeasurer, model.Background, useVerticalTextAlignmentWorkaround))
            {
                model.Update(true);
                model.Render(rc, new OxyRect(0, 0, width, height));
                rc.Complete();
                rc.Flush();
            }
        }

        public static string ExportToString(IPlotModel model, double width, double height, bool isDocument, IRenderContext textMeasurer = null, bool useVerticalTextAlignmentWorkaround = false)
        {
            string svg;
            using (var ms = new MemoryStream())
            {
                Export(model, ms, width, height, isDocument, textMeasurer, useVerticalTextAlignmentWorkaround);
                ms.Flush();
                ms.Position = 0;
                var sr = new StreamReader(ms);
                svg = sr.ReadToEnd();
            }

            return svg;
        }

        public void Export(IPlotModel model, Stream stream)
        {
            Export(model, stream, this.Width, this.Height, this.IsDocument, this.TextMeasurer, this.UseVerticalTextAlignmentWorkaround);
        }

        public string ExportToString(IPlotModel model)
        {
            return ExportToString(model, this.Width, this.Height, this.IsDocument, this.TextMeasurer, this.UseVerticalTextAlignmentWorkaround);
        }
    }
}
