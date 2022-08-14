namespace OxyPlot.Series
{
    using System;

    public class RectangleItem : ICodeGenerating, IEquatable<RectangleItem>
    {
        public static readonly RectangleItem Undefined = new RectangleItem(DataPoint.Undefined, DataPoint.Undefined, double.NaN);
        public RectangleItem(double x1, double x2, double y1, double y2, double value)
        {
            this.A = new DataPoint(x1, y1);
            this.B = new DataPoint(x2, y2);
            this.Value = value;
        }

        public RectangleItem(DataPoint a, DataPoint b, double value)
        {
            this.A = a;
            this.B = b;
            this.Value = value;
        }


        public DataPoint A { get; }
        public DataPoint B { get; }
        public double Value { get; }
        public bool Contains(DataPoint p)
        {
            return (p.X <= this.B.X && p.X >= this.A.X && p.Y <= this.B.Y && p.Y >= this.A.Y) ||
                   (p.X <= this.A.X && p.X >= this.B.X && p.Y <= this.A.Y && p.Y >= this.B.Y);
        }

        public string ToCode()
        {
            return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1},{2}", this.A, this.B, this.Value);
        }

        public bool Equals(RectangleItem other)
        {
            return this.A.Equals(other.A) && this.B.Equals(other.B);
        }
        public override string ToString()
        {
            return $"{this.A} {this.B} {this.Value}";
        }

        public bool IsDefined()
        {
#pragma warning disable 1718
            return this.A.IsDefined() && this.B.IsDefined() && !double.IsNaN(this.Value);
#pragma warning restore 1718
        }
    }
}
