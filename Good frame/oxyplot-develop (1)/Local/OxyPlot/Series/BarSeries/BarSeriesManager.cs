namespace OxyPlot.Series
{
    using OxyPlot.Axes;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class BarSeriesManager
    {
        private double[] currentBarOffset;
        private double[,] currentMaxValue;
        private double[,] currentMinValue;
        private double[,] currentNegativeBaseValues;
        private double[,] currentPositiveBaseValues;
        private double maxWidth;
        public BarSeriesManager(CategoryAxis categoryAxis, Axis valueAxis, IEnumerable<IBarSeries> series)
        {
            this.PlotModel = categoryAxis.PlotModel ?? throw new InvalidOperationException("The category axis must be part of a plot model.");
            this.CategoryAxis = categoryAxis;
            this.ValueAxis = valueAxis;
            this.ManagedSeries = series.ToList();

            foreach (var s in this.ManagedSeries)
            {
                s.Manager = this;
            }
        }

        public CategoryAxis CategoryAxis { get; }
        public IList<IBarSeries> ManagedSeries { get; }
        public PlotModel PlotModel { get; }
        public Axis ValueAxis { get; }
        internal IList<string> Categories => this.CategoryAxis.ActualLabels;
        private double[] BarOffset { get; set; }
        private double[,] StackedBarOffset { get; set; }
        private Dictionary<string, int> StackIndexMapping { get; } = new Dictionary<string, int>();
        public double GetCategoryValue(int categoryIndex, int stackIndex, double actualBarWidth)
        {
            var offsetBegin = this.StackedBarOffset[stackIndex, categoryIndex];
            var offsetEnd = this.StackedBarOffset[stackIndex + 1, categoryIndex];
            return categoryIndex - 0.5 + ((offsetEnd + offsetBegin - actualBarWidth) * 0.5);
        }

        public double GetCurrentBarOffset(int categoryIndex)
        {
            return this.currentBarOffset[categoryIndex];
        }

        public double GetCurrentBaseValue(int stackIndex, int categoryIndex, bool negativeValue)
        {
            return negativeValue ? this.currentNegativeBaseValues[stackIndex, categoryIndex] : this.currentPositiveBaseValues[stackIndex, categoryIndex];
        }

        public double GetCurrentMaxValue(int stackIndex, int categoryIndex)
        {
            return this.currentMaxValue[stackIndex, categoryIndex];
        }

        public double GetCurrentMinValue(int stackIndex, int categoryIndex)
        {
            return this.currentMinValue[stackIndex, categoryIndex];
        }

        public double GetMaxWidth()
        {
            return this.maxWidth;
        }

        public int GetStackIndex(string stackGroup)
        {
            return this.StackIndexMapping[stackGroup];
        }


        public void IncreaseCurrentBarOffset(int categoryIndex, double delta)
        {
            this.currentBarOffset[categoryIndex] += delta;
        }

        public void InitializeRender()
        {
            this.ResetCurrentValues();
        }

        public void SetCurrentBaseValue(int stackIndex, int categoryIndex, bool negativeValue, double newValue)
        {
            if (negativeValue)
            {
                this.currentNegativeBaseValues[stackIndex, categoryIndex] = newValue;
            }
            else
            {
                this.currentPositiveBaseValues[stackIndex, categoryIndex] = newValue;
            }
        }

        public void SetCurrentMaxValue(int stackIndex, int categoryIndex, double newValue)
        {
            this.currentMaxValue[stackIndex, categoryIndex] = newValue;
        }

        public void SetCurrentMinValue(int stackIndex, int categoryIndex, double newValue)
        {
            this.currentMinValue[stackIndex, categoryIndex] = newValue;
        }

        public void Update()
        {
            this.CategoryAxis.UpdateLabels(this.ManagedSeries.Max(s => s.ActualItems.Count));
            this.UpdateBarOffsets();
            this.UpdateValidData();
            this.ResetCurrentValues();
        }

        private static bool HasCategory(IBarSeries series, int categoryIndex)
        {
            if (series.ActualItems.Count > categoryIndex && series.ActualItems[categoryIndex].CategoryIndex < 0)
            {
                return true;
            }

            return series.ActualItems.Any(item => item.CategoryIndex == categoryIndex);
        }

        private void ResetCurrentValues()
        {
            this.currentBarOffset = this.BarOffset?.ToArray();
            var actualLabels = this.CategoryAxis.ActualLabels;
            if (this.StackIndexMapping.Count > 0)
            {
                this.currentPositiveBaseValues = new double[this.StackIndexMapping.Count, actualLabels.Count];
                this.currentPositiveBaseValues.Fill2D(double.NaN);
                this.currentNegativeBaseValues = new double[this.StackIndexMapping.Count, actualLabels.Count];
                this.currentNegativeBaseValues.Fill2D(double.NaN);

                this.currentMaxValue = new double[this.StackIndexMapping.Count, actualLabels.Count];
                this.currentMaxValue.Fill2D(double.NaN);
                this.currentMinValue = new double[this.StackIndexMapping.Count, actualLabels.Count];
                this.currentMinValue.Fill2D(double.NaN);
            }
            else
            {
                this.currentPositiveBaseValues = null;
                this.currentNegativeBaseValues = null;
                this.currentMaxValue = null;
                this.currentMinValue = null;
            }
        }

        private void UpdateBarOffsets()
        {
            if (this.Categories.Count == 0)
            {
                this.maxWidth = double.NaN;
                this.StackedBarOffset = null;
                this.StackIndexMapping.Clear();

                return;
            }

            var totalWidthPerCategory = new double[this.Categories.Count];
            var stackGroupWidthDict = new Dictionary<string, double>();
            this.BarOffset = new double[this.Categories.Count];

            var stackGroups = this.ManagedSeries
                .OfType<IStackableSeries>()
                .Where(s => s.IsStacked)
                .GroupBy(s => s.StackGroup)
                .ToList();

            foreach (var stackGroup in stackGroups)
            {
                var groupList = stackGroup.ToList();
                var maxBarWidth = groupList
                    .Select(s => s.BarWidth)
                    .MaxOrDefault(0);

                stackGroupWidthDict.Add(stackGroup.Key, maxBarWidth);

                for (var i = 0; i < this.Categories.Count; i++)
                {
                    if (groupList.Any(s => HasCategory(s, i)))
                    {
                        totalWidthPerCategory[i] += maxBarWidth;
                    }
                }
            }

            foreach (var s in this.ManagedSeries.Where(s => !(s is IStackableSeries stackable) || !stackable.IsStacked))
            {
                for (var i = 0; i < this.Categories.Count; i++)
                {
                    var numberOfItems = s.ActualItems.Count(item => item.CategoryIndex == i);
                    if (s.ActualItems.Count > i && s.ActualItems[i].CategoryIndex < 0)
                    {
                        numberOfItems++;
                    }

                    totalWidthPerCategory[i] += s.BarWidth * numberOfItems;
                }
            }

            this.maxWidth = totalWidthPerCategory.Max();

            // Calculate BarOffset and StackedBarOffset
            this.StackedBarOffset = new double[stackGroups.Count + 1, this.Categories.Count];

            var widthScale = 1 / (1 + this.CategoryAxis.GapWidth) / this.maxWidth;
            for (var i = 0; i < this.Categories.Count; i++)
            {
                this.BarOffset[i] = 0.5 - (totalWidthPerCategory[i] * widthScale * 0.5);
            }

            for (var j = 0; j < stackGroups.Count; j++)
            {
                var stackGroup = stackGroups[j];
                var groupList = stackGroup.ToList();
                for (var i = 0; i < this.Categories.Count; i++)
                {
                    this.StackedBarOffset[j, i] = this.BarOffset[i];
                    if (groupList.Any(s => HasCategory(s, i)))
                    {
                        this.BarOffset[i] += stackGroupWidthDict[stackGroup.Key] * widthScale;
                    }
                }
            }

            for (var i = 0; i < this.Categories.Count; i++)
            {
                this.StackedBarOffset[stackGroups.Count, i] = this.BarOffset[i];
            }

            this.StackIndexMapping.Clear();
            var groupCounter = 0;
            foreach (var group in stackGroups.Select(group => group.Key).OrderBy(key => key))
            {
                this.StackIndexMapping.Add(group, groupCounter++);
            }
        }

        private void UpdateValidData()
        {
            foreach (var item in this.ManagedSeries)
            {
                item.UpdateValidData();
            }
        }
    }
}
