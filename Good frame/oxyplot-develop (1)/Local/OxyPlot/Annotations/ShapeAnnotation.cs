namespace OxyPlot.Annotations
{
    public abstract class ShapeAnnotation : TextualAnnotation
    {
        protected ShapeAnnotation()
        {
            this.Stroke = OxyColors.Black;
            this.Fill = OxyColors.LightBlue;
        }
        
        public OxyColor Fill { get; set; }
        public OxyColor Stroke { get; set; }
        public double StrokeThickness { get; set; }
    }
}
