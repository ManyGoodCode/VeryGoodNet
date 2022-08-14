namespace OxyPlot
{
    public class PortableDocumentFontFamily
    {
        public PortableDocumentFont RegularFont { get; set; }
        public PortableDocumentFont BoldFont { get; set; }
        public PortableDocumentFont ItalicFont { get; set; }
        public PortableDocumentFont BoldItalicFont { get; set; }
        public PortableDocumentFont GetFont(bool bold, bool italic)
        {
            if (bold && italic && this.BoldItalicFont != null)
            {
                return this.BoldItalicFont;
            }

            if (bold && this.BoldFont != null)
            {
                return this.BoldFont;
            }

            if (italic && this.ItalicFont != null)
            {
                return this.ItalicFont;
            }

            return this.RegularFont;
        }
    }
}