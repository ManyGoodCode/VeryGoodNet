namespace OxyPlot
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public abstract class PlotElement : Element, IPlotElement
    {
        protected PlotElement()
        {
            this.Font = null;
            this.FontSize = double.NaN;
            this.FontWeight = FontWeights.Normal;
            this.TextColor = OxyColors.Automatic;
            this.EdgeRenderingMode = EdgeRenderingMode.Automatic;
        }

        public string Font { get; set; }

        public double FontSize { get; set; }

        public double FontWeight { get; set; }

        public PlotModel PlotModel
        {
            get
            {
                return (PlotModel)this.Parent;
            }
        }

        public object Tag { get; set; }

        public OxyColor TextColor { get; set; }

        public EdgeRenderingMode EdgeRenderingMode { get; set; }
        public string ToolTip { get; set; }

        protected internal string ActualFont
        {
            get
            {
                return this.Font ?? this.PlotModel.DefaultFont;
            }
        }

        protected internal double ActualFontSize
        {
            get
            {
                return !double.IsNaN(this.FontSize) ? this.FontSize : this.PlotModel.DefaultFontSize;
            }
        }

        protected internal double ActualFontWeight
        {
            get
            {
                return this.FontWeight;
            }
        }

        protected internal OxyColor ActualTextColor
        {
            get
            {
                return this.TextColor.GetActualColor(this.PlotModel.TextColor);
            }
        }

        protected CultureInfo ActualCulture
        {
            get
            {
                return this.PlotModel != null ? this.PlotModel.ActualCulture : CultureInfo.CurrentCulture;
            }
        }

        public virtual OxyRect GetClippingRect()
        {
            return OxyRect.Everything;
        }

        public virtual int GetElementHashCode()
        {
            IEnumerable<PropertyInfo> properties = this.GetType().GetRuntimeProperties().Where(pi => pi.GetMethod.IsPublic && !pi.GetMethod.IsStatic);

            IEnumerable<object> propertyValues = properties.Select(pi => pi.GetValue(this, null));
            return HashCodeBuilder.GetHashCode(propertyValues);
        }
    }
}
