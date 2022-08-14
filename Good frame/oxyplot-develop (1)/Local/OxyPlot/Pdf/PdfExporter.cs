namespace OxyPlot
{
    using System;
    using System.IO;

    [Obsolete("OxyPlot.PdfExporter may be removed in a future version. Consider using OxyPlot.SkiaSharp.PdfExporter instead.")]
    public class PdfExporter : IExporter
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public static void Export(
            IPlotModel model,
            Stream stream, 
            double width, 
            double height)
        {
            PdfExporter exporter = new PdfExporter
            { 
                Width = width,
                Height = height 
            };
            exporter.Export(model, stream);
        }

        public void Export(IPlotModel model, Stream stream)
        {
            PdfRenderContext rc = new PdfRenderContext(
                this.Width, 
                this.Height, 
                model.Background);
            model.Update(true);
            model.Render(rc, new OxyRect(0, 0, this.Width, this.Height));
            rc.Save(stream);
        }
    }
}
