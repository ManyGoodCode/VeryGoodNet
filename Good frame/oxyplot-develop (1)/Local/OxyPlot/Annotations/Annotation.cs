
namespace OxyPlot.Annotations
{
    using OxyPlot.Axes;

    public abstract class Annotation : PlotElement, IXyAxisPlotElement
    {
        protected Annotation()
        {
            this.Layer = AnnotationLayer.AboveSeries;
            this.ClipByXAxis = true;
            this.ClipByYAxis = true;
        }

        public AnnotationLayer Layer { get; set; }

        public Axis XAxis { get; private set; }

        public string XAxisKey { get; set; }

        public Axis YAxis { get; private set; }
        
        public bool ClipByXAxis { get; set; }

        public bool ClipByYAxis { get; set; }

        public string YAxisKey { get; set; }

        public void EnsureAxes()
        {
            this.XAxis = this.XAxisKey != null ? this.PlotModel.GetAxis(this.XAxisKey) : this.PlotModel.DefaultXAxis;
            this.YAxis = this.YAxisKey != null ? this.PlotModel.GetAxis(this.YAxisKey) : this.PlotModel.DefaultYAxis;
        }


        public virtual void Render(IRenderContext rc)
        {
        }

        public override OxyRect GetClippingRect()
        {
            OxyRect rect = this.PlotModel.PlotArea;
            OxyRect axisRect = PlotElementUtilities.GetClippingRect(this);

            double minX = 0d;
            double maxX = double.PositiveInfinity;
            double minY = 0d;
            double maxY = double.PositiveInfinity;

            if (this.ClipByXAxis)
            {
                minX = axisRect.TopLeft.X;
                maxX = axisRect.BottomRight.X;
            }

            if (this.ClipByYAxis)
            {
                minY = axisRect.TopLeft.Y;
                maxY = axisRect.BottomRight.Y;
            }

            ScreenPoint minPoint = new ScreenPoint(minX, minY);
            ScreenPoint maxPoint = new ScreenPoint(maxX, maxY);

            OxyRect axisClipRect = new OxyRect(minPoint, maxPoint);
            return rect.Clip(axisClipRect);
        }

        public virtual ScreenPoint Transform(DataPoint p)
        {
            return PlotElementUtilities.Transform(this, p);
        }

        public virtual DataPoint InverseTransform(ScreenPoint p)
        {
            return PlotElementUtilities.InverseTransform(this, p);
        }
    }
}
