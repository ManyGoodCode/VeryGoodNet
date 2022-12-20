
namespace OxyPlot.Axes
{
    public class LinearAxis : Axis
    {
        public LinearAxis()
        {
            this.FractionUnit = 1.0;
            this.FractionUnitSymbol = null;
            this.FormatAsFractions = false;
        }

        public bool FormatAsFractions { get; set; }
        public double FractionUnit { get; set; }
        public string FractionUnitSymbol { get; set; }
        public override bool IsXyAxis()
        {
            return true;
        }

        public override bool IsLogarithmic()
        {
            return false;
        }
        
        protected override string FormatValueOverride(double x)
        {
            if (this.FormatAsFractions)
            {
                return FractionHelper.ConvertToFractionString(x, this.FractionUnit, this.FractionUnitSymbol, 1e-6, this.ActualCulture, this.StringFormat);
            }

            return base.FormatValueOverride(x);
        }
    }
}
