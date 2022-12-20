
namespace OxyPlot.Axes
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    public class CategoryAxis : LinearAxis
    {
        private readonly List<string> autoGeneratedLabels = new List<string>();
        private readonly List<string> itemsSourceLabels = new List<string>();
        public CategoryAxis()
        {
            this.TickStyle = TickStyle.Outside;
            this.Position = AxisPosition.Bottom;
            this.MinimumPadding = 0;
            this.MaximumPadding = 0;
            this.MajorStep = 1;
            this.GapWidth = 1;
        }

        public IList<string> ActualLabels
        {
            get
            {
                if (this.ItemsSource != null)
                {
                    return this.itemsSourceLabels;
                }

                if (this.Labels.Count > 0)
                {
                    return this.Labels;
                }

                return this.autoGeneratedLabels;
            }
        }
        
        public double GapWidth { get; set; }
        public bool IsTickCentered { get; set; }
        public IEnumerable ItemsSource { get; set; }
        public string LabelField { get; set; }
        public List<string> Labels { get; } = new List<string>();

        public override void GetTickValues(
            out IList<double> majorLabelValues, out IList<double> majorTickValues, out IList<double> minorTickValues)
        {
            base.GetTickValues(out majorLabelValues, out majorTickValues, out minorTickValues);
            minorTickValues.Clear();

            if (!this.IsTickCentered)
            {
                const double epsilon = 1e-3;

                var mv = new List<double>(majorLabelValues.Count + 1);
                mv.AddRange(majorLabelValues.Select(v => v - 0.5).Where(v => v > this.ClipMinimum - epsilon));

                if (mv.Count > 0)
                {
                    var lastTick = mv[mv.Count - 1] + 1;
                    if (lastTick < this.ClipMaximum + epsilon)
                    {
                        mv.Add(lastTick);
                    }
                }

                majorTickValues = mv;
            }
        }

        public override object GetValue(double x)
        {
            return this.FormatValue(x);
        }

        internal override void UpdateActualMaxMin()
        {
            this.Include(-0.5);

            var actualLabels = this.ActualLabels;

            if (actualLabels.Count > 0)
            {
                this.Include((actualLabels.Count - 1) + 0.5);
            }
            else
            {
                this.Include(0.5);
            }

            base.UpdateActualMaxMin();

            this.MinorStep = 1;
        }

        protected internal void UpdateLabels(int numberOfCategories)
        {
            if (this.ItemsSource != null)
            {
                this.itemsSourceLabels.Clear();
                this.itemsSourceLabels.AddRange(this.ItemsSource.Format(this.LabelField, this.StringFormat, this.ActualCulture));
                return;
            }

            if (this.Labels.Count == 0)
            {
                if (this.autoGeneratedLabels.Count == numberOfCategories)
                {
                    return;
                }

                this.autoGeneratedLabels.Clear();
                this.autoGeneratedLabels.AddRange(Enumerable.Range(1, numberOfCategories).Select(number => number.ToString(CultureInfo.InvariantCulture)));
            }
        }

        protected override string FormatValueOverride(double x)
        {
            var index = (int)x;
            var actualLabels = this.ActualLabels;
            if (index >= 0 && index < actualLabels.Count)
            {
                return actualLabels[index];
            }

            return null;
        }
    }
}
