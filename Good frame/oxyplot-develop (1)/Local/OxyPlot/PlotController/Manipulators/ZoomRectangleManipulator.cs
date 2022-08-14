namespace OxyPlot
{
    using System;

    public class ZoomRectangleManipulator : MouseManipulator
    {
        private OxyRect zoomRectangle;

        public ZoomRectangleManipulator(IPlotView plotView)
            : base(plotView)
        {
        }

        private bool IsZoomEnabled { get; set; }

        public override void Completed(OxyMouseEventArgs e)
        {
            base.Completed(e);
            if (!this.IsZoomEnabled)
            {
                return;
            }

            this.PlotView.SetCursorType(CursorType.Default);
            this.PlotView.HideZoomRectangle();
            
            if (this.zoomRectangle.Width > 10 && this.zoomRectangle.Height > 10)
            {
                DataPoint p0 = this.InverseTransform(this.zoomRectangle.Left, this.zoomRectangle.Top);
                DataPoint p1 = this.InverseTransform(this.zoomRectangle.Right, this.zoomRectangle.Bottom);

                if (this.XAxis != null)
                {
                    this.XAxis.Zoom(p0.X, p1.X);
                }

                if (this.YAxis != null)
                {
                    this.YAxis.Zoom(p0.Y, p1.Y);
                }

                this.PlotView.InvalidatePlot();
            }

            e.Handled = true;
        }

        public override void Delta(OxyMouseEventArgs e)
        {
            base.Delta(e);
            if (!this.IsZoomEnabled)
            {
                return;
            }

            OxyRect plotArea = this.PlotView.ActualModel.PlotArea;

            double x = Math.Min(this.StartPosition.X, e.Position.X);
            double w = Math.Abs(this.StartPosition.X - e.Position.X);
            double y = Math.Min(this.StartPosition.Y, e.Position.Y);
            double h = Math.Abs(this.StartPosition.Y - e.Position.Y);

            if (this.XAxis == null || !this.XAxis.IsZoomEnabled)
            {
                x = plotArea.Left;
                w = plotArea.Width;
            }

            if (this.YAxis == null || !this.YAxis.IsZoomEnabled)
            {
                y = plotArea.Top;
                h = plotArea.Height;
            }

            this.zoomRectangle = new OxyRect(x, y, w, h);
            this.PlotView.ShowZoomRectangle(this.zoomRectangle);
            e.Handled = true;
        }

        public override void Started(OxyMouseEventArgs e)
        {
            base.Started(e);

            this.IsZoomEnabled = (this.XAxis != null && this.XAxis.IsZoomEnabled)
                     || (this.YAxis != null && this.YAxis.IsZoomEnabled);

            if (this.IsZoomEnabled)
            {
                this.zoomRectangle = new OxyRect(this.StartPosition.X, this.StartPosition.Y, 0, 0);
                this.PlotView.ShowZoomRectangle(this.zoomRectangle);
                this.PlotView.SetCursorType(this.GetCursorType());
                e.Handled = true;
            }
        }

        private CursorType GetCursorType()
        {
            if (this.XAxis == null)
            {
                return CursorType.ZoomVertical;
            }

            if (this.YAxis == null)
            {
                return CursorType.ZoomHorizontal;
            }

            return CursorType.ZoomRectangle;
        }
    }
}