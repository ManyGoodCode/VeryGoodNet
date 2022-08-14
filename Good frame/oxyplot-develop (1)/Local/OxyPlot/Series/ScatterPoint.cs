namespace OxyPlot.Series
{
    public class ScatterPoint : ICodeGenerating
    {
        public ScatterPoint(double x, double y, double size = double.NaN, double value = double.NaN, object tag = null)
        {
            this.X = x;
            this.Y = y;
            this.Size = size;
            this.Value = value;
            this.Tag = tag;
        }

        public double X { get; private set; }
        public double Y { get; private set; }
        public double Size { get; set; }
        public double Value { get; set; }
        public object Tag { get; set; }
        public virtual string ToCode()
        {
            if (double.IsNaN(this.Size) && double.IsNaN(this.Value))
            {
                return CodeGenerator.FormatConstructor(this.GetType(), "{0}, {1}", this.X, this.Y);
            }

            if (double.IsNaN(this.Value))
            {
                return CodeGenerator.FormatConstructor(this.GetType(), "{0}, {1}, {2}", this.X, this.Y, this.Size);
            }

            return CodeGenerator.FormatConstructor(
                this.GetType(), "{0}, {1}, {2}, {3}", this.X, this.Y, this.Size, this.Value);
        }

        public override string ToString()
        {
            return this.X + " " + this.Y;
        }
    }
}