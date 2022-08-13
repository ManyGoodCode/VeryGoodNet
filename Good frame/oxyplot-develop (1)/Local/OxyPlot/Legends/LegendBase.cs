namespace OxyPlot.Legends
{
    public enum LegendPlacement
    {
        Inside,
        Outside
    }

    public enum LegendPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
        LeftTop,
        LeftMiddle,
        LeftBottom,
        RightTop,
        RightMiddle,
        RightBottom
    }

    public enum LegendOrientation
    {
        Horizontal,
        Vertical
    }

    public enum LegendItemOrder
    {
        Normal,
        Reverse
    }

    public enum LegendSymbolPlacement
    {
        Left,
        Right
    }

    public abstract class LegendBase : PlotElement
    {
        protected override HitTestResult HitTestOverride(HitTestArguments args)
        {
            return this.LegendHitTest(args);
        }

        protected abstract HitTestResult LegendHitTest(HitTestArguments args);
        public string Key { get; set; }

        public bool IsLegendVisible { get; set; }
        public LegendOrientation LegendOrientation { get; set; }
        public double LegendPadding { get; set; }
        public double LegendSymbolLength { get; set; }
        public double LegendSymbolMargin { get; set; }
        public LegendSymbolPlacement LegendSymbolPlacement { get; set; }
        public string LegendTitle { get; set; }

        public OxyColor LegendTitleColor { get; set; }
        public string LegendTitleFont { get; set; }
        public double LegendTitleFontSize { get; set; }
        public double LegendTitleFontWeight { get; set; }
        public OxyRect LegendArea { get; set; }
        public OxySize LegendSize { get; set; }
        public OxyColor LegendBackground { get; set; }
        public OxyColor LegendBorder { get; set; }
        public double LegendBorderThickness { get; set; }

        public double LegendColumnSpacing { get; set; }
        public string LegendFont { get; set; }

        public double LegendFontSize { get; set; }
        public OxyColor LegendTextColor { get; set; }
        public double LegendFontWeight { get; set; }
        public HorizontalAlignment LegendItemAlignment { get; set; }
        public LegendItemOrder LegendItemOrder { get; set; }

        public double LegendItemSpacing { get; set; }
        public double LegendLineSpacing { get; set; }

        public double LegendMargin { get; set; }
        public double LegendMaxWidth { get; set; }
        public double LegendMaxHeight { get; set; }
        public LegendPlacement LegendPlacement { get; set; }
        public LegendPosition LegendPosition { get; set; }
        public bool AllowUseFullExtent { get; set; }
        public bool ShowInvisibleSeries { get; set; }
        public abstract void EnsureLegendProperties();
        public abstract OxySize GetLegendSize(IRenderContext rc, OxySize availableLegendArea);

        public abstract OxyRect GetLegendRectangle(OxySize legendSize);

        public abstract void RenderLegends(IRenderContext rc);
    }
}
