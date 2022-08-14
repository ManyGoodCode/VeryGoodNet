namespace OxyPlot.Series
{
    using System;


    public struct DataRange : ICodeGenerating
    {
        public static readonly DataRange Undefined = default;

        private readonly double minimum;
        private readonly double maximum;
        private readonly bool isDefined;

        public DataRange(double min, double max)
        {
            if (double.IsNaN(min) || double.IsNaN(max))
            {
                throw new ArgumentException("NaN values are not permitted");
            }

            if (max < min)
            {
                throw new ArgumentException("max must be larger or equal min");
            }

            this.minimum = min;
            this.maximum = max;

            this.isDefined = true;
        }

        public double Minimum => this.minimum;
        public double Maximum => this.maximum;
        public double Range => this.Maximum - this.Minimum;
        public bool IsDefined()
        {
            return this.isDefined;
        }

        public bool Contains(double value)
        {
            return value >= this.Minimum && value <= this.Maximum;
        }

        public bool IntersectsWith(DataRange other)
        {
            return (this.IsDefined() && other.IsDefined()) &&
                   (this.Contains(other.Minimum) ||
                    this.Contains(other.Maximum) ||
                    other.Contains(this.Minimum) ||
                    other.Contains(this.Maximum));
        }

        public string ToCode()
        {
            return CodeGenerator.FormatConstructor(this.GetType(), "{0},{1}", this.Minimum, this.Maximum);
        }

        public override string ToString()
        {
            return $"[{this.Minimum}, {this.Maximum}]";
        }
    }
}
