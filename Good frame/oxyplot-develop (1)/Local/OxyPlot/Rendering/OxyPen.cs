namespace OxyPlot
{
    using System;


    public class OxyPen
    {
        public OxyPen(
            OxyColor color,
            double thickness = 1.0,
            LineStyle lineStyle = LineStyle.Solid,
            LineJoin lineJoin = LineJoin.Miter)
        {
            this.Color = color;
            this.Thickness = thickness;
            this.DashArray = lineStyle.GetDashArray();
            this.LineStyle = lineStyle;
            this.LineJoin = lineJoin;
        }

        public OxyColor Color { get; set; }
        public double[] DashArray { get; set; }
        public LineJoin LineJoin { get; set; }
        public LineStyle LineStyle { get; set; }
        public double Thickness { get; set; }
        public double[] ActualDashArray
        {
            get
            {
                return this.DashArray ?? this.LineStyle.GetDashArray();
            }
        }

        public static OxyPen Create(
            OxyColor color,
            double thickness,
            LineStyle lineStyle = LineStyle.Solid,
            LineJoin lineJoin = LineJoin.Miter)
        {
            if (color.IsInvisible() || lineStyle == LineStyle.None || Math.Abs(thickness) < double.Epsilon)
            {
                return null;
            }

            return new OxyPen(color, thickness, lineStyle, lineJoin);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.Color.GetHashCode();
                result = (result * 397) ^ this.Thickness.GetHashCode();
                result = (result * 397) ^ this.LineStyle.GetHashCode();
                result = (result * 397) ^ this.LineJoin.GetHashCode();
                return result;
            }
        }
    }
}