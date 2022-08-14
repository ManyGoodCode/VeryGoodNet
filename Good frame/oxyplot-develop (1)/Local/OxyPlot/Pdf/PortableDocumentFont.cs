namespace OxyPlot
{
    public class PortableDocumentFont
    {
        public PortableDocumentFont()
        {
            this.FirstChar = 0;
            this.Encoding = FontEncoding.WinAnsiEncoding;
        }

        public FontSubType SubType { get; set; }
        public string BaseFont { get; set; }
        public FontEncoding Encoding { get; set; }
        public int FirstChar { get; set; }
        public int[] Widths { get; set; }
        public int Ascent { get; set; }
        public int CapHeight { get; set; }
        public int Descent { get; set; }
        public int Flags { get; set; }
        public int[] FontBoundingBox { get; set; }
        public int ItalicAngle { get; set; }
        public int StemV { get; set; }
        public int XHeight { get; set; }

        public string FontName { get; set; }
        public void Measure(
            string text,
            double fontSize, 
            out double width, 
            out double height)
        {
            int wmax = 0;
            string[] lines = StringHelper.SplitLines(text);
            int lineCount = lines.Length;
            foreach (string line in lines)
            {
                int w = 0;
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if (c >= this.FirstChar + this.Widths.Length)
                    {
                        continue;
                    }

                    w += this.Widths[c - this.FirstChar];
                }

                if (w > wmax)
                {
                    wmax = w;
                }
            }

            width = wmax * fontSize / 1000;
            height = lineCount * (this.Ascent - this.Descent) * fontSize / 1000;
        }
    }
}
