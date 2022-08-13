namespace OxyPlot.Legends
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public partial class Legend
    {
        public override void EnsureLegendProperties()
        {
            switch (this.LegendPosition)
            {
                case LegendPosition.LeftTop:
                case LegendPosition.LeftMiddle:
                case LegendPosition.LeftBottom:
                case LegendPosition.RightTop:
                case LegendPosition.RightMiddle:
                case LegendPosition.RightBottom:
                    if (this.LegendOrientation == LegendOrientation.Horizontal)
                    {
                        this.LegendOrientation = LegendOrientation.Vertical;
                    }

                    break;
            }
        }

        public override void RenderLegends(IRenderContext rc)
        {
            this.RenderOrMeasureLegends(rc, this.LegendArea);
        }

        public override OxySize GetLegendSize(IRenderContext rc, OxySize availableLegendArea)
        {
            double availableLegendWidth = availableLegendArea.Width;
            double availableLegendHeight = availableLegendArea.Height;

            OxySize legendSize = this.MeasureLegends(rc, new OxySize(Math.Max(0, availableLegendWidth), Math.Max(0, availableLegendHeight)));
            legendSize = new OxySize(Math.Max(0, legendSize.Width), Math.Max(0, legendSize.Height));
            return legendSize;
        }

        public override OxyRect GetLegendRectangle(OxySize legendSize)
        {
            double top = 0;
            double left = 0;
            if (this.LegendPlacement == LegendPlacement.Outside)
            {
                switch (this.LegendPosition)
                {
                    case LegendPosition.LeftTop:
                    case LegendPosition.LeftMiddle:
                    case LegendPosition.LeftBottom:
                        left = this.PlotModel.PlotAndAxisArea.Left - legendSize.Width - this.LegendMargin;
                        break;
                    case LegendPosition.RightTop:
                    case LegendPosition.RightMiddle:
                    case LegendPosition.RightBottom:
                        left = this.PlotModel.PlotAndAxisArea.Right + this.LegendMargin;
                        break;
                    case LegendPosition.TopLeft:
                    case LegendPosition.TopCenter:
                    case LegendPosition.TopRight:
                        top = this.PlotModel.PlotAndAxisArea.Top - legendSize.Height - this.LegendMargin;
                        break;
                    case LegendPosition.BottomLeft:
                    case LegendPosition.BottomCenter:
                    case LegendPosition.BottomRight:
                        top = this.PlotModel.PlotAndAxisArea.Bottom + this.LegendMargin;
                        break;
                }

                OxyRect bounds = this.AllowUseFullExtent
                    ? this.PlotModel.PlotAndAxisArea
                    : this.PlotModel.PlotArea;

                switch (this.LegendPosition)
                {
                    case LegendPosition.TopLeft:
                    case LegendPosition.BottomLeft:
                        left = bounds.Left;
                        break;
                    case LegendPosition.TopRight:
                    case LegendPosition.BottomRight:
                        left = bounds.Right - legendSize.Width;
                        break;
                    case LegendPosition.LeftTop:
                    case LegendPosition.RightTop:
                        top = bounds.Top;
                        break;
                    case LegendPosition.LeftBottom:
                    case LegendPosition.RightBottom:
                        top = bounds.Bottom - legendSize.Height;
                        break;
                    case LegendPosition.LeftMiddle:
                    case LegendPosition.RightMiddle:
                        top = (bounds.Top + bounds.Bottom - legendSize.Height) * 0.5;
                        break;
                    case LegendPosition.TopCenter:
                    case LegendPosition.BottomCenter:
                        left = (bounds.Left + bounds.Right - legendSize.Width) * 0.5;
                        break;
                }
            }
            else
            {
                switch (this.LegendPosition)
                {
                    case LegendPosition.LeftTop:
                    case LegendPosition.LeftMiddle:
                    case LegendPosition.LeftBottom:
                        left = this.PlotModel.PlotArea.Left + this.LegendMargin;
                        break;
                    case LegendPosition.RightTop:
                    case LegendPosition.RightMiddle:
                    case LegendPosition.RightBottom:
                        left = this.PlotModel.PlotArea.Right - legendSize.Width - this.LegendMargin;
                        break;
                    case LegendPosition.TopLeft:
                    case LegendPosition.TopCenter:
                    case LegendPosition.TopRight:
                        top = this.PlotModel.PlotArea.Top + this.LegendMargin;
                        break;
                    case LegendPosition.BottomLeft:
                    case LegendPosition.BottomCenter:
                    case LegendPosition.BottomRight:
                        top = this.PlotModel.PlotArea.Bottom - legendSize.Height - this.LegendMargin;
                        break;
                }

                switch (this.LegendPosition)
                {
                    case LegendPosition.TopLeft:
                    case LegendPosition.BottomLeft:
                        left = this.PlotModel.PlotArea.Left + this.LegendMargin;
                        break;
                    case LegendPosition.TopRight:
                    case LegendPosition.BottomRight:
                        left = this.PlotModel.PlotArea.Right - legendSize.Width - this.LegendMargin;
                        break;
                    case LegendPosition.LeftTop:
                    case LegendPosition.RightTop:
                        top = this.PlotModel.PlotArea.Top + this.LegendMargin;
                        break;
                    case LegendPosition.LeftBottom:
                    case LegendPosition.RightBottom:
                        top = this.PlotModel.PlotArea.Bottom - legendSize.Height - this.LegendMargin;
                        break;

                    case LegendPosition.LeftMiddle:
                    case LegendPosition.RightMiddle:
                        top = (this.PlotModel.PlotArea.Top + this.PlotModel.PlotArea.Bottom - legendSize.Height) * 0.5;
                        break;
                    case LegendPosition.TopCenter:
                    case LegendPosition.BottomCenter:
                        left = (this.PlotModel.PlotArea.Left + this.PlotModel.PlotArea.Right - legendSize.Width) * 0.5;
                        break;
                }
            }

            return new OxyRect(left, top, legendSize.Width, legendSize.Height);
        }


        private void RenderLegend(IRenderContext rc, Series.Series s, OxyRect rect)
        {
            HorizontalAlignment actualItemAlignment = this.LegendItemAlignment;
            if (this.LegendOrientation == LegendOrientation.Horizontal)
            {
                actualItemAlignment = HorizontalAlignment.Left;
            }

            double x = rect.Left;
            switch (actualItemAlignment)
            {
                case HorizontalAlignment.Center:
                    x = (rect.Left + rect.Right) / 2;
                    if (this.LegendSymbolPlacement == LegendSymbolPlacement.Left)
                    {
                        x -= (this.LegendSymbolLength + this.LegendSymbolMargin) / 2;
                    }
                    else
                    {
                        x -= (this.LegendSymbolLength + this.LegendSymbolMargin) / 2;
                    }

                    break;
                case HorizontalAlignment.Right:
                    x = rect.Right;
                    x -= this.LegendSymbolLength + this.LegendSymbolMargin;
                    break;
            }

            if (this.LegendSymbolPlacement == LegendSymbolPlacement.Left)
            {
                x += this.LegendSymbolLength + this.LegendSymbolMargin;
            }

            double y = rect.Top;
            OxySize maxsize = new OxySize(Math.Max(rect.Width - this.LegendSymbolLength - this.LegendSymbolMargin, 0), rect.Height);
            double actualLegendFontSize = double.IsNaN(this.LegendFontSize) ? this.PlotModel.DefaultFontSize : this.LegendFontSize;
            OxyColor legendTextColor = s.IsVisible ? this.LegendTextColor : this.SeriesInvisibleTextColor;

            rc.SetToolTip(s.ToolTip);
            OxySize textSize = rc.DrawMathText(
                new ScreenPoint(x, y),
                s.Title,
                legendTextColor.GetActualColor(this.PlotModel.TextColor),
                this.LegendFont ?? this.PlotModel.DefaultFont,
                actualLegendFontSize,
                this.LegendFontWeight,
                0,
                actualItemAlignment,
                VerticalAlignment.Top,
                maxsize,
                true);

            this.SeriesPosMap.Add(s, new OxyRect(new ScreenPoint(x, y), textSize));
            double x0 = x;
            switch (actualItemAlignment)
            {
                case HorizontalAlignment.Center:
                    x0 = x - (textSize.Width * 0.5);
                    break;
                case HorizontalAlignment.Right:
                    x0 = x - textSize.Width;
                    break;
            }

            if (s.IsVisible)
            {
                OxyRect symbolRect =
                    new OxyRect(
                        this.LegendSymbolPlacement == LegendSymbolPlacement.Right
                            ? x0 + textSize.Width + this.LegendSymbolMargin
                            : x0 - this.LegendSymbolMargin - this.LegendSymbolLength,
                        rect.Top,
                        this.LegendSymbolLength,
                        textSize.Height);

                s.RenderLegend(rc, symbolRect);
            }

            rc.SetToolTip(null);
        }

        private OxySize MeasureLegends(IRenderContext rc, OxySize availableSize)
        {
            return this.RenderOrMeasureLegends(rc, new OxyRect(0, 0, availableSize.Width, availableSize.Height), true);
        }

        private OxySize RenderOrMeasureLegends(IRenderContext rc, OxyRect rect, bool measureOnly = false)
        {
            if (!measureOnly && rect.Width > 0 && rect.Height > 0)
            {
                this.legendBox = rect;
                rc.DrawRectangle(
                    rect, 
                    this.LegendBackground, 
                    this.LegendBorder, 
                    this.LegendBorderThickness, 
                    this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));
            }

            double availableWidth = rect.Width;
            double availableHeight = rect.Height;

            double x = this.LegendPadding;
            double top = this.LegendPadding;

            OxySize size = new OxySize();

            double actualLegendFontSize = double.IsNaN(this.LegendFontSize) ? this.PlotModel.DefaultFontSize : this.LegendFontSize;
            double actualLegendTitleFontSize = double.IsNaN(this.LegendTitleFontSize) ? actualLegendFontSize : this.LegendTitleFontSize;
            double actualGroupNameFontSize = double.IsNaN(this.GroupNameFontSize) ? actualLegendFontSize : this.GroupNameFontSize;

            if (!string.IsNullOrEmpty(this.LegendTitle))
            {
                OxySize titleSize;
                if (measureOnly)
                {
                    titleSize = rc.MeasureMathText(
                        this.LegendTitle,
                        this.LegendTitleFont ?? this.PlotModel.DefaultFont,
                        actualLegendTitleFontSize,
                        this.LegendTitleFontWeight);
                }
                else
                {
                    titleSize = rc.DrawMathText(
                        new ScreenPoint(rect.Left + x, rect.Top + top),
                        this.LegendTitle,
                        this.LegendTitleColor.GetActualColor(this.PlotModel.TextColor),
                        this.LegendTitleFont ?? this.PlotModel.DefaultFont,
                        actualLegendTitleFontSize,
                        this.LegendTitleFontWeight,
                        0,
                        HorizontalAlignment.Left,
                        VerticalAlignment.Top,
                        null,
                        true);
                }

                top += titleSize.Height;
                size = new OxySize(x + titleSize.Width + this.LegendPadding, top + titleSize.Height);
            }

            double y = top;

            double lineHeight = 0;
            const double Epsilon = 1e-3;
            double maxItemWidth = 0;

            IEnumerable<Series.Series> items = this.LegendItemOrder == LegendItemOrder.Reverse
                ? this.PlotModel.Series.Reverse().Where(s => s.RenderInLegend && s.LegendKey == this.Key)
                : this.PlotModel.Series.Where(s => s.RenderInLegend && s.LegendKey == this.Key);

            List<string> itemGroupNames = new List<string>();
            foreach (Series.Series s in items)
            {
                if (!itemGroupNames.Contains(s.SeriesGroupName))
                {
                    itemGroupNames.Add(s.SeriesGroupName);
                }
            }

            this.SeriesPosMap.Clear();
            Dictionary<Series.Series, OxyRect> seriesToRender = new Dictionary<Series.Series, OxyRect>();
            Action renderItems = () =>
            {
                List<string> usedGroupNames = new List<string>();
                foreach (KeyValuePair<Series.Series,OxyRect> sr in seriesToRender)
                {
                    OxyRect itemRect = sr.Value;
                    Series.Series itemSeries = sr.Key;

                    if (!string.IsNullOrEmpty(itemSeries.SeriesGroupName) && !usedGroupNames.Contains(itemSeries.SeriesGroupName))
                    {
                        usedGroupNames.Add(itemSeries.SeriesGroupName);
                        OxySize groupNameTextSize = rc.MeasureMathText(itemSeries.SeriesGroupName, this.GroupNameFont ?? this.PlotModel.DefaultFont, actualGroupNameFontSize, this.GroupNameFontWeight);
                        double ypos = itemRect.Top;
                        double xpos = itemRect.Left;
                        if (this.LegendOrientation == LegendOrientation.Vertical)
                            ypos -= (groupNameTextSize.Height + this.LegendLineSpacing / 2);
                        else
                            xpos -= (groupNameTextSize.Width + this.LegendItemSpacing / 2);
                        rc.DrawMathText(
                        new ScreenPoint(xpos, ypos),
                        itemSeries.SeriesGroupName,
                        this.LegendTitleColor.GetActualColor(this.PlotModel.TextColor),
                         this.GroupNameFont ?? this.PlotModel.DefaultFont,
                        actualGroupNameFontSize,
                        this.GroupNameFontWeight,
                        0,
                        HorizontalAlignment.Left,
                        VerticalAlignment.Top,
                        null,
                        true);
                    }

                    double rwidth = availableWidth;
                    if (itemRect.Left + rwidth + this.LegendPadding > rect.Left + availableWidth)
                    {
                        rwidth = rect.Left + availableWidth - itemRect.Left - this.LegendPadding;
                    }

                    double rheight = itemRect.Height;
                    if (rect.Top + rheight + this.LegendPadding > rect.Top + availableHeight)
                    {
                        rheight = rect.Top + availableHeight - rect.Top - this.LegendPadding;
                    }

                    OxyRect r = new OxyRect(itemRect.Left, itemRect.Top, Math.Max(rwidth, 0), Math.Max(rheight, 0));
                    this.RenderLegend(rc, itemSeries, r);
                }

                usedGroupNames.Clear();
                seriesToRender.Clear();
            };

            if (!measureOnly)
            {
                rc.PushClip(rect);
            }

            foreach (string g in itemGroupNames)
            {
                IEnumerable<Series.Series> itemGroup = items.Where(i => i.SeriesGroupName == g);
                OxySize groupNameTextSize = new OxySize(0, 0);
                if (itemGroup.Count() > 0 && !string.IsNullOrEmpty(g))
                {
                    groupNameTextSize = rc.MeasureMathText(g, this.GroupNameFont ?? this.PlotModel.DefaultFont, actualGroupNameFontSize, this.GroupNameFontWeight);
                    if (this.LegendOrientation == LegendOrientation.Vertical)
                        y += groupNameTextSize.Height;
                    else
                        x += groupNameTextSize.Width;
                }

                int count = 0;
                foreach (Series.Series s in itemGroup)
                {
                    if (string.IsNullOrEmpty(s.Title) || !s.RenderInLegend)
                    {
                        continue;
                    }

                    if (!s.IsVisible && !this.ShowInvisibleSeries)
                    {
                        continue;
                    }

                    OxySize textSize = rc.MeasureMathText(s.Title, this.LegendFont ?? this.PlotModel.DefaultFont, actualLegendFontSize, this.LegendFontWeight);
                    double itemWidth = this.LegendSymbolLength + this.LegendSymbolMargin + textSize.Width;
                    double itemHeight = textSize.Height;

                    if (this.LegendOrientation == LegendOrientation.Horizontal)
                    {
                        // Add spacing between items
                        if (x > this.LegendPadding)
                        {
                            x += this.LegendItemSpacing;
                        }

                        if (x + itemWidth > availableWidth - this.LegendPadding + Epsilon)
                        {
                            x = this.LegendPadding;
                            if (count == 0 && groupNameTextSize.Width > 0)
                                x += (groupNameTextSize.Width + this.LegendItemSpacing);
                            y += lineHeight + this.LegendLineSpacing;
                            lineHeight = 0;
                        }

                        lineHeight = Math.Max(lineHeight, textSize.Height);

                        if (!measureOnly)
                        {
                            seriesToRender.Add(s, new OxyRect(rect.Left + x, rect.Top + y, itemWidth, itemHeight));
                        }

                        x += itemWidth;

                        x = Math.Max(groupNameTextSize.Width, x);
                        size = new OxySize(Math.Max(size.Width, x), Math.Max(size.Height, y + textSize.Height));
                    }
                    else
                    {
                        if (y + itemHeight > availableHeight - this.LegendPadding + Epsilon)
                        {
                            renderItems();

                            y = top + groupNameTextSize.Height;
                            x += maxItemWidth + this.LegendColumnSpacing;
                            maxItemWidth = 0;
                        }

                        if (!measureOnly)
                        {
                            seriesToRender.Add(s, new OxyRect(rect.Left + x, rect.Top + y, itemWidth, itemHeight));
                        }

                        y += itemHeight + this.LegendLineSpacing;

                        maxItemWidth = Math.Max(maxItemWidth, itemWidth);
                        size = new OxySize(Math.Max(size.Width, x + itemWidth), Math.Max(size.Height, y));
                    }

                    count++;
                }

                renderItems();
            }

            if (!measureOnly)
            {
                rc.PopClip();
            }

            if (size.Width > 0)
            {
                size = new OxySize(size.Width + this.LegendPadding, size.Height);
            }

            if (size.Height > 0)
            {
                size = new OxySize(size.Width, size.Height + this.LegendPadding);
            }

            if (size.Width > availableWidth)
            {
                size = new OxySize(availableWidth, size.Height);
            }

            if (size.Height > availableHeight)
            {
                size = new OxySize(size.Width, availableHeight);
            }

            if (!double.IsNaN(this.LegendMaxWidth) && size.Width > this.LegendMaxWidth)
            {
                size = new OxySize(this.LegendMaxWidth, size.Height);
            }

            if (!double.IsNaN(this.LegendMaxHeight) && size.Height > this.LegendMaxHeight)
            {
                size = new OxySize(size.Width, this.LegendMaxHeight);
            }

            return size;
        }
    }
}
