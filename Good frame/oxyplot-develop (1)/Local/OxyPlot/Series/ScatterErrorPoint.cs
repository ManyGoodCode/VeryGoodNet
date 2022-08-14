namespace OxyPlot.Series
{
    public class ScatterErrorPoint : ScatterPoint
    {
        public ScatterErrorPoint(double x, double y, double errorX, double errorY, double size = double.NaN, double value = double.NaN, object tag = null)
            : base(x, y, size, value, tag)
        {
            this.ErrorX = errorX;
            this.ErrorY = errorY;
        }

        public double ErrorX { get; private set; }
        public double ErrorY { get; private set; }

        public override string ToCode()
        {
            if (double.IsNaN(this.Size) && double.IsNaN(this.Value))
            {
                return CodeGenerator.FormatConstructor(this.GetType(), "{0}, {1}, {2}, {3}", this.X, this.Y, this.ErrorX, this.ErrorY);
            }

            if (double.IsNaN(this.Value))
            {
                return CodeGenerator.FormatConstructor(this.GetType(), "{0}, {1}, {2}, {3}, {4}", this.X, this.Y, this.ErrorX, this.ErrorY, this.Size);
            }

            return CodeGenerator.FormatConstructor(
                this.GetType(), "{0}, {1}, {2}, {3}, {3}, {4}, {5}", this.X, this.Y, this.ErrorX, this.ErrorY, this.Size, this.Value);
        }
    }
}