namespace OxyPlot.Annotations
{
    using System;

    public abstract class TextualAnnotation : TransposableAnnotation
    {
        protected TextualAnnotation()
        {
            this.TextHorizontalAlignment = HorizontalAlignment.Center;
            this.TextVerticalAlignment = VerticalAlignment.Middle;
            this.TextPosition = DataPoint.Undefined;
            this.TextRotation = 0;
        }
        
        public string Text { get; set; }
        public DataPoint TextPosition { get; set; }
        public HorizontalAlignment TextHorizontalAlignment { get; set; }
        public VerticalAlignment TextVerticalAlignment { get; set; }
        public double TextRotation { get; set; }
        protected ScreenPoint GetActualTextPosition(Func<ScreenPoint> defaultPosition)
        {
            return this.TextPosition.IsDefined() ? this.Transform(this.TextPosition) : defaultPosition();
        }
        
        protected void GetActualTextAlignment(out HorizontalAlignment ha, out VerticalAlignment va)
        {
            ha = this.TextHorizontalAlignment;
            va = this.TextVerticalAlignment;
        }
    }
}
