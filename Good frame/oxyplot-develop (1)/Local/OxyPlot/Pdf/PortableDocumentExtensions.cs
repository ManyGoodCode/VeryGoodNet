namespace OxyPlot
{
    public static class PortableDocumentExtensions
    {
        public static void SetColor(this PortableDocument doc, OxyColor c)
        {
            doc.SetColor(c.R / 255.0, c.G / 255.0, c.B / 255.0);
            doc.SetStrokeAlpha(c.A / 255.0);
        }

        public static void SetFillColor(this PortableDocument doc, OxyColor c)
        {
            doc.SetFillColor(c.R / 255.0, c.G / 255.0, c.B / 255.0);
            doc.SetFillAlpha(c.A / 255.0);
        }
    }
}