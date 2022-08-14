namespace OxyPlot.Series
{
    public class RectangleBarItem : ICodeGenerating
    {
        public RectangleBarItem()
        {
            this.Color = OxyColors.Automatic;
        }

        public RectangleBarItem(double x0, double y0, double x1, double y1)
            : this()
        {
            this.X0 = x0;
            this.Y0 = y0;
            this.X1 = x1;
            this.Y1 = y1;
        }

        public OxyColor Color { get; set; }
        public string Title { get; set; }
        public double X0 { get; set; }
        public double X1 { get; set; }
        public double Y0 { get; set; }
        public double Y1 { get; set; }
        public string ToCode()
        {
            if (!this.Color.IsUndefined())
            {
                return CodeGenerator.FormatConstructor(
                    this.GetType(),
                    "{0},{1},{2},{3},{4},{5}",
                    this.X0,
                    this.Y0,
                    this.X1,
                    this.Y1,
                    this.Title,
                    this.Color.ToCode());
            }

            if (this.Title != null)
            {
                return CodeGenerator.FormatConstructor(
                    this.GetType(), "{0},{1},{2},{3},{4}", this.X0, this.Y0, this.X1, this.Y1, this.Title);
            }

            return CodeGenerator.FormatConstructor(
                this.GetType(), "{0},{1},{2},{3}", this.X0, this.Y0, this.X1, this.Y1);
        }
    }
}