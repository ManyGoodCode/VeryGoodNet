namespace OxyPlot
{
    using System;

    public struct PlotLength : IEquatable<PlotLength>
    {
        private readonly PlotLengthUnit unit;

        private readonly double value;

        public PlotLength(double value, PlotLengthUnit unit)
        {
            this.value = value;
            this.unit = unit;
        }

        public double Value
        {
            get { return this.value; }
        }

        public PlotLengthUnit Unit
        {
            get{  return this.unit;}
        }

        public bool Equals(PlotLength other)
        {
            return this.value.Equals(other.value) && this.unit.Equals(other.unit);
        }
    }
}