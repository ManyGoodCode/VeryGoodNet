namespace OxyPlot.Series
{
    public abstract class BarItemBase
    {
        protected BarItemBase()
        {
            this.CategoryIndex = -1;
        }

        public int CategoryIndex { get; set; }

        internal int GetCategoryIndex(int defaultIndex)
        {
            if (this.CategoryIndex < 0)
            {
                return defaultIndex;
            }

            return this.CategoryIndex;
        }
    }
}
