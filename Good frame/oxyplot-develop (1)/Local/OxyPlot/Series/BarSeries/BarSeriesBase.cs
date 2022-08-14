namespace OxyPlot.Series
{
    using OxyPlot.Axes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class BarSeriesBase<T> : XYAxisSeries, IBarSeries where T : BarItemBase
    {
        protected const string DefaultCategoryAxisTitle = "Category";
        protected const string DefaultValueAxisTitle = "Value";
        private bool ownsItemsSourceItems;
        protected BarSeriesBase()
        {
            this.StrokeColor = OxyColors.Black;
            this.BarWidth = 1;
        }

        public List<T> ActualItems => this.ItemsSource != null ? this.ItemsSourceItems : this.Items;

        IReadOnlyList<BarItemBase> IBarSeries.ActualItems => this.ActualItems;

        public double BarWidth { get; set; }

        CategoryAxis IBarSeries.CategoryAxis => this.GetCategoryAxis();

        public List<T> Items { get; } = new List<T>();

        Axis IBarSeries.ValueAxis => this.XAxis;

        protected List<T> ItemsSourceItems { get; set; }

        BarSeriesManager IBarSeries.Manager { get => this.Manager; set => this.Manager = value; }

        public OxyColor StrokeColor { get; set; }

        public double StrokeThickness { get; set; }
        protected BarSeriesManager Manager { get; set; }
        protected IList<T> ValidItems { get; } = new List<T>();
        protected Dictionary<int, int> ValidItemsIndexInversion { get; } = new Dictionary<int, int>();
        protected double GetActualBarWidth()
        {
            var categoryAxis = this.GetCategoryAxis();
            return this.BarWidth / (1 + categoryAxis.GapWidth) / this.Manager.GetMaxWidth();
        }

        protected CategoryAxis GetCategoryAxis()
        {
            if (!(this.YAxis is CategoryAxis ca))
            {
                throw new Exception("BarSeries requires a CategoryAxis on the Y Axis.");
            }

            return ca;
        }

        protected override object GetItem(int i)
        {
            if (this.ItemsSource != null || this.ActualItems == null || this.ActualItems.Count == 0)
            {
                return base.GetItem(i);
            }

            return this.ActualItems[i];
        }

        protected abstract bool IsValid(T item);
        protected abstract bool UpdateFromDataFields();
        private void ClearItemsSourceItems()
        {
            if (!this.ownsItemsSourceItems || this.ItemsSourceItems == null)
            {
                this.ItemsSourceItems = new List<T>();
            }
            else
            {
                this.ItemsSourceItems.Clear();
            }

            this.ownsItemsSourceItems = true;
        }

        void IBarSeries.UpdateValidData()
        {
            this.UpdateValidData();
        }

        protected internal override bool IsUsing(Axis axis)
        {
            return this.XAxis == axis || this.YAxis == axis;
        }

        protected internal override void UpdateAxisMaxMin()
        {
            this.XAxis.Include(this.MinX);
            this.XAxis.Include(this.MaxX);
        }

        protected internal override void UpdateData()
        {
            if (this.ItemsSource is List<T> lst)
            {
                this.ItemsSourceItems = lst;
                this.ownsItemsSourceItems = false;
            }
            else
            {
                this.ClearItemsSourceItems();
                if (this.ItemsSource != null && !this.UpdateFromDataFields())
                {
                    this.ItemsSourceItems.AddRange(this.ItemsSource.OfType<T>());
                }
            }
        }

        protected void UpdateValidData()
        {
            this.ValidItems.Clear();
            this.ValidItemsIndexInversion.Clear();
            var numberOfCategories = this.Manager.Categories.Count;
            var valueAxis = this.XAxis;

            var i = 0;
            var items = this.ActualItems
                .Where(item => item.GetCategoryIndex(i) < numberOfCategories)
                .Where(this.IsValid);

            foreach (var item in items)
            {
                this.ValidItemsIndexInversion.Add(this.ValidItems.Count, i);
                this.ValidItems.Add(item);
                i++;
            }
        }
    }
}
