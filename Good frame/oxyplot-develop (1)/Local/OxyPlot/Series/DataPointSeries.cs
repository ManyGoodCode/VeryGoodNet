namespace OxyPlot.Series
{
    using System;
    using System.Collections.Generic;

    public abstract class DataPointSeries : XYAxisSeries
    {
        private readonly List<DataPoint> points = new List<DataPoint>();
        private List<DataPoint> itemsSourcePoints;
        private bool ownsItemsSourcePoints;
        public bool CanTrackerInterpolatePoints { get; set; }
        public string DataFieldX { get; set; }
        public string DataFieldY { get; set; }
        public Func<object, DataPoint> Mapping { get; set; }
        public List<DataPoint> Points
        {
            get
            {
                return this.points;
            }
        }

        protected List<DataPoint> ActualPoints
        {
            get
            {
                return this.ItemsSource != null ? this.itemsSourcePoints : this.points;
            }
        }

        public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
        {
            if (interpolate && !this.CanTrackerInterpolatePoints)
            {
                return null;
            }

            TrackerHitResult result = null;
            if (interpolate)
            {
                result = this.GetNearestInterpolatedPointInternal(this.ActualPoints, point);
            }

            if (result == null)
            {
                result = this.GetNearestPointInternal(this.ActualPoints, point);
            }

            if (result != null)
            {
                result.Text = StringHelper.Format(
                    this.ActualCulture, 
                    this.TrackerFormatString,
                    result.Item,
                    this.Title,
                    this.XAxis.Title ?? XYAxisSeries.DefaultXAxisTitle,
                    this.XAxis.GetValue(result.DataPoint.X),
                    this.YAxis.Title ?? XYAxisSeries.DefaultYAxisTitle,
                    this.YAxis.GetValue(result.DataPoint.Y));
            }

            return result;
        }

        protected internal override void UpdateData()
        {
            if (this.ItemsSource == null)
            {
                return;
            }

            this.UpdateItemsSourcePoints();
        }

        protected internal override void UpdateMaxMin()
        {
            base.UpdateMaxMin();
            this.InternalUpdateMaxMin(this.ActualPoints);
        }
        protected override object GetItem(int i)
        {
            var actualPoints = this.ActualPoints;
            if (this.ItemsSource == null && actualPoints != null && i < actualPoints.Count)
            {
                return actualPoints[i];
            }

            return base.GetItem(i);
        }

        private void ClearItemsSourcePoints()
        {
            if (!this.ownsItemsSourcePoints || this.itemsSourcePoints == null)
            {
                this.itemsSourcePoints = new List<DataPoint>();
            }
            else
            {
                this.itemsSourcePoints.Clear();
            }

            this.ownsItemsSourcePoints = true;
        }

        private void UpdateItemsSourcePoints()
        {
            if (this.Mapping != null)
            {
                this.ClearItemsSourcePoints();
                foreach (var item in this.ItemsSource)
                {
                    this.itemsSourcePoints.Add(this.Mapping(item));
                }

                return;
            }

            var sourceAsListOfDataPoints = this.ItemsSource as List<DataPoint>;
            if (sourceAsListOfDataPoints != null)
            {
                this.itemsSourcePoints = sourceAsListOfDataPoints;
                this.ownsItemsSourcePoints = false;
                return;
            }

            this.ClearItemsSourcePoints();

            var sourceAsEnumerableDataPoints = this.ItemsSource as IEnumerable<DataPoint>;
            if (sourceAsEnumerableDataPoints != null)
            {
                this.itemsSourcePoints.AddRange(sourceAsEnumerableDataPoints);
                return;
            }

            if (this.DataFieldX == null || this.DataFieldY == null)
            {
                foreach (var item in this.ItemsSource)
                {
                    if (item is DataPoint)
                    {
                        this.itemsSourcePoints.Add((DataPoint)item);
                        continue;
                    }

                    var idpp = item as IDataPointProvider;
                    if (idpp == null)
                    {
                        continue;
                    }

                    this.itemsSourcePoints.Add(idpp.GetDataPoint());
                }
            }
            else
            {
                var filler = new ListBuilder<DataPoint>();
                filler.Add(this.DataFieldX, double.NaN);
                filler.Add(this.DataFieldY, double.NaN);
                filler.Fill(this.itemsSourcePoints, this.ItemsSource, args => new DataPoint(Axes.Axis.ToDouble(args[0]), Axes.Axis.ToDouble(args[1])));
            }
        }
    }
}