namespace OxyPlot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot.Annotations;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    using OxyPlot.Legends;

    public partial class PlotModel
    {
        void IPlotModel.Render(IRenderContext rc, OxyRect rect)
        {
            this.RenderOverride(rc, rect);
        }

        protected virtual void RenderOverride(IRenderContext rc, OxyRect rect)
        {
            lock (this.SyncRoot)
            {
                var initialClipCount = rc.ClipCount;

                try
                {
                    using var _ = rc.AutoResetClip(rect);
                    if (this.lastPlotException != null)
                    {
                        string errorMessage = string.Format(
                                "An exception of type {0} was thrown when updating the plot model.\r\n{1}",
                                this.lastPlotException.GetType(),
                                this.lastPlotException.GetBaseException().StackTrace);
                        this.RenderErrorMessage(rc, string.Format("OxyPlot exception: {0}", this.lastPlotException.Message), errorMessage);
                        return;
                    }

                    if (this.RenderingDecorator != null)
                    {
                        rc = this.RenderingDecorator(rc);
                    }

                    this.PlotBounds = rect;

                    this.ActualPlotMargins =
                        new OxyThickness(
                            double.IsNaN(this.PlotMargins.Left) ? 0 : this.PlotMargins.Left,
                            double.IsNaN(this.PlotMargins.Top) ? 0 : this.PlotMargins.Top,
                            double.IsNaN(this.PlotMargins.Right) ? 0 : this.PlotMargins.Right,
                            double.IsNaN(this.PlotMargins.Bottom) ? 0 : this.PlotMargins.Bottom);

                    foreach (LegendBase l in this.Legends)
                    {
                        l.EnsureLegendProperties();
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        this.UpdatePlotArea(rc);
                        this.UpdateAxisTransforms();
                        this.UpdateIntervals();

                        if (!this.AdjustPlotMargins(rc))
                        {
                            break;
                        }
                    }

                    if (this.PlotType == PlotType.Cartesian)
                    {
                        this.EnforceCartesianTransforms();
                        this.UpdateIntervals();
                    }

                    this.RenderBackgrounds(rc);
                    this.RenderAnnotations(rc, AnnotationLayer.BelowAxes);
                    this.RenderAxes(rc, AxisLayer.BelowSeries);
                    this.RenderAnnotations(rc, AnnotationLayer.BelowSeries);
                    this.RenderSeries(rc);
                    this.RenderAnnotations(rc, AnnotationLayer.AboveSeries);
                    this.RenderTitle(rc);
                    this.RenderBox(rc);
                    this.RenderAxes(rc, AxisLayer.AboveSeries);

                    if (this.IsLegendVisible)
                    {
                        this.RenderLegends(rc);
                    }

                    if (rc.ClipCount != initialClipCount + 1)
                    {
                        throw new InvalidOperationException("Unbalanced calls to IRenderContext.PushClip were made during rendering.");
                    }
                }
                catch (Exception exception)
                {
                    while (rc.ClipCount > initialClipCount)
                    {
                        rc.PopClip();
                    }

                    string errorMessage = string.Format(
                            "An exception of type {0} was thrown when rendering the plot model.\r\n{1}",
                            exception.GetType(),
                            exception.GetBaseException().StackTrace);
                    this.lastPlotException = exception;
                    this.RenderErrorMessage(rc, string.Format("OxyPlot exception: {0}", exception.Message), errorMessage);
                }
                finally
                {
                    rc.CleanUp();
                }
            }
        }

        private void RenderErrorMessage(IRenderContext rc, string title, string errorMessage, double fontSize = 12)
        {
            ScreenPoint p0 = new ScreenPoint(10, 10);
            rc.DrawText(p0, title, this.TextColor, fontWeight: FontWeights.Bold, fontSize: fontSize);
            rc.DrawMultilineText(p0 + new ScreenVector(0, fontSize * 1.5), errorMessage, this.TextColor, fontSize: fontSize, dy: fontSize * 1.25);
        }


        private bool AdjustPlotMargins(IRenderContext rc)
        {
            List<Axis> visibleAxes = this.Axes.Where(axis => axis.IsAxisVisible).ToList();
            foreach (Axis axis in visibleAxes)
            {
                axis.Measure(rc);
            }

            OxyThickness desiredMargin = new OxyThickness();

            void IncludeInMargin(double size, AxisPosition borderPosition)
            {
                desiredMargin = borderPosition switch
                {
                    AxisPosition.Bottom => new OxyThickness(desiredMargin.Left, desiredMargin.Top, desiredMargin.Right, Math.Max(desiredMargin.Bottom, size)),
                    AxisPosition.Left => new OxyThickness(Math.Max(desiredMargin.Left, size), desiredMargin.Top, desiredMargin.Right, desiredMargin.Bottom),
                    AxisPosition.Right => new OxyThickness(desiredMargin.Left, desiredMargin.Top, Math.Max(desiredMargin.Right, size), desiredMargin.Bottom),
                    AxisPosition.Top => new OxyThickness(desiredMargin.Left, Math.Max(desiredMargin.Top, size), desiredMargin.Right, desiredMargin.Bottom),
                    _ => desiredMargin,
                };
            }

            for (AxisPosition position = AxisPosition.Left; position <= AxisPosition.Bottom; position++)
            {
                var axesOfPosition = visibleAxes.Where(a => a.Position == position);
                var requiredSize = this.AdjustAxesPositions(axesOfPosition);
                IncludeInMargin(requiredSize, position);
            }

            foreach (var axis in visibleAxes)
            {
                desiredMargin = desiredMargin.Include(axis.DesiredMargin);
            }

            var currentMargin = this.PlotMargins;
            currentMargin = new OxyThickness(
                double.IsNaN(currentMargin.Left) ? desiredMargin.Left : currentMargin.Left,
                double.IsNaN(currentMargin.Top) ? desiredMargin.Top : currentMargin.Top,
                double.IsNaN(currentMargin.Right) ? desiredMargin.Right : currentMargin.Right,
                double.IsNaN(currentMargin.Bottom) ? desiredMargin.Bottom : currentMargin.Bottom);

            if (currentMargin.Equals(this.ActualPlotMargins))
            {
                return false;
            }

            this.ActualPlotMargins = currentMargin;
            return true;
        }

        private double AdjustAxesPositions(IEnumerable<Axis> parallelAxes)
        {
            double maxValueOfPositionTier = 0;

            static double GetSize(Axis axis)
            {
                return axis.Position switch
                {
                    AxisPosition.Left => axis.DesiredMargin.Left,
                    AxisPosition.Right => axis.DesiredMargin.Right,
                    AxisPosition.Top => axis.DesiredMargin.Top,
                    AxisPosition.Bottom => axis.DesiredMargin.Bottom,
                    _ => throw new InvalidOperationException(), // we don't do this for polar axes
                };
            }

            foreach (var tierGroup in parallelAxes.GroupBy(a => a.PositionTier).OrderBy(group => group.Key))
            {
                var axesOfPositionTier = tierGroup.ToList();
                var maxSizeOfPositionTier = axesOfPositionTier.Max(GetSize);

                var minValueOfPositionTier = maxValueOfPositionTier;

                if (Math.Abs(maxValueOfPositionTier) > 1e-5)
                {
                    maxValueOfPositionTier += this.AxisTierDistance;
                }

                maxValueOfPositionTier += maxSizeOfPositionTier;

                foreach (var axis in axesOfPositionTier)
                {
                    axis.PositionTierSize = maxSizeOfPositionTier;
                    axis.PositionTierMinShift = minValueOfPositionTier;
                    axis.PositionTierMaxShift = maxValueOfPositionTier;
                }
            }

            return maxValueOfPositionTier;
        }

        private OxySize MeasureTitles(IRenderContext rc)
        {
            var titleSize = rc.MeasureText(this.Title, this.ActualTitleFont, this.TitleFontSize, this.TitleFontWeight);
            var subtitleSize = rc.MeasureText(this.Subtitle, this.SubtitleFont ?? this.ActualSubtitleFont, this.SubtitleFontSize, this.SubtitleFontWeight);
            double height = titleSize.Height + subtitleSize.Height;
            double width = Math.Max(titleSize.Width, subtitleSize.Width);
            return new OxySize(width, height);
        }

        private void RenderAnnotations(IRenderContext rc, AnnotationLayer layer)
        {
            this.RenderPlotElements(this.Annotations.Where(a => a.Layer == layer), rc, annotation => annotation.Render(rc));
        }

        private void RenderAxes(IRenderContext rc, AxisLayer layer)
        {
            foreach (Axis a in this.Axes.Where(a => a.IsAxisVisible && a.Layer == layer))
            {
                rc.SetToolTip(a.ToolTip);
                a.Render(rc, 0);
            }

            foreach (Axis a in this.Axes.Where(a => a.IsAxisVisible && a.Layer == layer))
            {
                rc.SetToolTip(a.ToolTip);
                a.Render(rc, 1);
            }

            rc.SetToolTip(null);
        }

        private void RenderLegends(IRenderContext rc)
        {
            if (this.IsLegendVisible)
            {
                foreach (var l in this.Legends.Where(l => l.IsLegendVisible))
                {
                    rc.SetToolTip(l.ToolTip);
                    l.RenderLegends(rc);
                }
            }
        }

        private void RenderBackgrounds(IRenderContext rc)
        {
            if (this.Axes.Count > 0 && this.PlotAreaBackground.IsVisible())
            {
                rc.DrawRectangle(this.PlotArea, this.PlotAreaBackground, OxyColors.Undefined, 0, this.EdgeRenderingMode);
            }

            foreach (XYAxisSeries s in this.Series.Where(s => s.IsVisible && s is XYAxisSeries && s.Background.IsVisible()).Cast<XYAxisSeries>())
            {
                rc.DrawRectangle(s.GetScreenRectangle(), s.Background, OxyColors.Undefined, 0, this.EdgeRenderingMode);
            }
        }

        private void RenderBox(IRenderContext rc)
        {
            if (this.Axes.Count > 0)
            {
                rc.DrawRectangle(this.PlotArea, this.PlotAreaBorderColor, this.PlotAreaBorderThickness, this.EdgeRenderingMode.GetActual(EdgeRenderingMode.PreferSharpness));
            }
        }

        private void RenderSeries(IRenderContext rc)
        {
            foreach (BarSeriesManager barSeriesManager in this.barSeriesManagers)
            {
                barSeriesManager.InitializeRender();
            }

            this.RenderPlotElements(this.Series.Where(s => s.IsVisible), rc, series => series.Render(rc));
        }

        private void RenderPlotElements<T>(IEnumerable<T> plotElements, IRenderContext rc, Action<T> renderAction) where T: PlotElement
        {
            OxyRect previousClippingRect = OxyRect.Everything;

            foreach (T plotElement in plotElements)
            {
                OxyRect currentClippingRect = plotElement.GetClippingRect();
                if (!currentClippingRect.Equals(previousClippingRect))
                {
                    if (!previousClippingRect.Equals(OxyRect.Everything))
                    {
                        rc.PopClip();
                        previousClippingRect = OxyRect.Everything;
                    }

                    if (!currentClippingRect.Equals(OxyRect.Everything))
                    {
                        rc.PushClip(currentClippingRect);
                        previousClippingRect = currentClippingRect;
                    }
                }

                rc.SetToolTip(plotElement.ToolTip);
                renderAction(plotElement);
            }

            if (!previousClippingRect.Equals(OxyRect.Everything))
            {
                rc.PopClip();
            }

            rc.SetToolTip(null);
        }

        private void RenderTitle(IRenderContext rc)
        {
            OxySize? maxSize = null;

            if (this.ClipTitle)
            {
                maxSize = new OxySize(this.TitleArea.Width * this.TitleClippingLength, double.MaxValue);
            }

            OxySize titleSize = rc.MeasureText(this.Title, this.ActualTitleFont, this.TitleFontSize, this.TitleFontWeight);

            double x = (this.TitleArea.Left + this.TitleArea.Right) * 0.5;
            double y = this.TitleArea.Top;

            if (!string.IsNullOrEmpty(this.Title))
            {
                rc.SetToolTip(this.TitleToolTip);

                rc.DrawMathText(
                    new ScreenPoint(x, y),
                    this.Title,
                    this.TitleColor.GetActualColor(this.TextColor),
                    this.ActualTitleFont,
                    this.TitleFontSize,
                    this.TitleFontWeight,
                    0,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Top,
                    maxSize);
                y += titleSize.Height;

                rc.SetToolTip(null);
            }

            if (!string.IsNullOrEmpty(this.Subtitle))
            {
                rc.DrawMathText(
                    new ScreenPoint(x, y),
                    this.Subtitle,
                    this.SubtitleColor.GetActualColor(this.TextColor),
                    this.ActualSubtitleFont,
                    this.SubtitleFontSize,
                    this.SubtitleFontWeight,
                    0,
                    HorizontalAlignment.Center,
                    VerticalAlignment.Top,
                    maxSize);
            }
        }


        private void UpdatePlotArea(IRenderContext rc)
        {
            OxyRect plotAndAxisArea = new OxyRect(
                this.PlotBounds.Left + this.Padding.Left,
                this.PlotBounds.Top + this.Padding.Top,
                Math.Max(0, this.Width - this.Padding.Left - this.Padding.Right),
                Math.Max(0, this.Height - this.Padding.Top - this.Padding.Bottom));

            OxySize titleSize = this.MeasureTitles(rc);
            if (titleSize.Height > 0)
            {
                double titleHeight = titleSize.Height + this.TitlePadding;
                plotAndAxisArea = new OxyRect(plotAndAxisArea.Left, plotAndAxisArea.Top + titleHeight, plotAndAxisArea.Width, Math.Max(0, plotAndAxisArea.Height - titleHeight));
            }

            OxyRect plotArea = plotAndAxisArea.Deflate(this.ActualPlotMargins);
            if (this.IsLegendVisible)
            {
                OxySize maxLegendSize = new OxySize(0, 0);
                double legendMargin = 0;
                foreach (LegendBase legend in this.Legends.Where(l =>
                    l.LegendPlacement == LegendPlacement.Outside && (l.IsLegendVisible &&
                    (l.LegendPosition == LegendPosition.LeftTop || l.LegendPosition == LegendPosition.LeftMiddle || l.LegendPosition == LegendPosition.LeftBottom))))
                {
                    double availableLegendWidth = legend.AllowUseFullExtent
                        ? plotAndAxisArea.Width
                        : plotArea.Width;
                    double availableLegendHeight = legend.AllowUseFullExtent
                        ? plotAndAxisArea.Height
                        : plotArea.Height;
                    availableLegendHeight = double.IsNaN(legend.LegendMaxHeight) ?
                        availableLegendHeight : Math.Min(availableLegendHeight, legend.LegendMaxHeight);

                    OxySize lsiz = legend.GetLegendSize(rc, new OxySize(availableLegendWidth, availableLegendHeight));
                    legend.LegendSize = lsiz;
                    maxLegendSize = new OxySize(maxLegendSize.Width > lsiz.Width ? maxLegendSize.Width : lsiz.Width, maxLegendSize.Height > lsiz.Height ? maxLegendSize.Height : lsiz.Height);

                    if (legend.LegendMargin > legendMargin)
                        legendMargin = legend.LegendMargin;
                }

                if (maxLegendSize.Width > 0 || maxLegendSize.Height > 0)
                {
                    plotArea = new OxyRect(plotArea.Left + maxLegendSize.Width + legendMargin, plotArea.Top, Math.Max(0, plotArea.Width - (maxLegendSize.Width + legendMargin)), plotArea.Height);
                }

                maxLegendSize = new OxySize(0, 0);
                legendMargin = 0;
                foreach (LegendBase legend in this.Legends.Where(l =>
                    l.LegendPlacement == LegendPlacement.Outside && (l.IsLegendVisible &&
                    (l.LegendPosition == LegendPosition.RightTop || l.LegendPosition == LegendPosition.RightMiddle || l.LegendPosition == LegendPosition.RightBottom))))
                {
                    double availableLegendWidth = legend.AllowUseFullExtent
                        ? plotAndAxisArea.Width
                        : plotArea.Width;
                    double availableLegendHeight = legend.AllowUseFullExtent
                        ? plotAndAxisArea.Height
                        : plotArea.Height;
                    availableLegendHeight = double.IsNaN(legend.LegendMaxHeight) ?
                        availableLegendHeight : Math.Min(availableLegendHeight, legend.LegendMaxHeight);

                    OxySize lsiz = legend.GetLegendSize(rc, new OxySize(availableLegendWidth, availableLegendHeight));
                    legend.LegendSize = lsiz;
                    maxLegendSize = new OxySize(maxLegendSize.Width > lsiz.Width ? maxLegendSize.Width : lsiz.Width, maxLegendSize.Height > lsiz.Height ? maxLegendSize.Height : lsiz.Height);

                    if (legend.LegendMargin > legendMargin)
                        legendMargin = legend.LegendMargin;
                }

                if (maxLegendSize.Width > 0 || maxLegendSize.Height > 0)
                {
                    plotArea = new OxyRect(plotArea.Left, plotArea.Top, Math.Max(0, plotArea.Width - (maxLegendSize.Width + legendMargin)), plotArea.Height);
                }

                maxLegendSize = new OxySize(0, 0);
                legendMargin = 0;
                foreach (LegendBase legend in this.Legends.Where(l =>
                    l.LegendPlacement == LegendPlacement.Outside && (l.IsLegendVisible &&
                    (l.LegendPosition == LegendPosition.TopLeft || l.LegendPosition == LegendPosition.TopCenter || l.LegendPosition == LegendPosition.TopRight))))
                {
                    double availableLegendWidth = legend.AllowUseFullExtent
                        ? plotAndAxisArea.Width
                        : plotArea.Width;
                    double availableLegendHeight = legend.AllowUseFullExtent
                        ? plotAndAxisArea.Height
                        : plotArea.Height;
                    availableLegendHeight = double.IsNaN(legend.LegendMaxHeight) ?
                        availableLegendHeight : Math.Min(availableLegendHeight, legend.LegendMaxHeight);

                    OxySize lsiz = legend.GetLegendSize(rc, new OxySize(availableLegendWidth, availableLegendHeight));
                    legend.LegendSize = lsiz;
                    maxLegendSize = new OxySize(maxLegendSize.Width > lsiz.Width ? maxLegendSize.Width : lsiz.Width, maxLegendSize.Height > lsiz.Height ? maxLegendSize.Height : lsiz.Height);

                    if (legend.LegendMargin > legendMargin)
                        legendMargin = legend.LegendMargin;
                }

                if (maxLegendSize.Width > 0 || maxLegendSize.Height > 0)
                {
                    plotArea = new OxyRect(plotArea.Left, plotArea.Top + maxLegendSize.Height + legendMargin, plotArea.Width, Math.Max(0, plotArea.Height - (maxLegendSize.Height + legendMargin)));
                }

                maxLegendSize = new OxySize(0, 0);
                legendMargin = 0;
                foreach (LegendBase legend in this.Legends.Where(l =>
                    l.LegendPlacement == LegendPlacement.Outside && (l.IsLegendVisible &&
                    (l.LegendPosition == LegendPosition.BottomLeft || l.LegendPosition == LegendPosition.BottomCenter || l.LegendPosition == LegendPosition.BottomRight))))
                {
                    double availableLegendWidth = legend.AllowUseFullExtent
                        ? plotAndAxisArea.Width
                        : plotArea.Width;
                    double availableLegendHeight = legend.AllowUseFullExtent
                        ? plotAndAxisArea.Height
                        : plotArea.Height;
                    availableLegendHeight = double.IsNaN(legend.LegendMaxHeight) ?
                        availableLegendHeight : Math.Min(availableLegendHeight, legend.LegendMaxHeight);

                    OxySize lsiz = legend.GetLegendSize(rc, new OxySize(availableLegendWidth, availableLegendHeight));
                    legend.LegendSize = lsiz;
                    maxLegendSize = new OxySize(maxLegendSize.Width > lsiz.Width ? maxLegendSize.Width : lsiz.Width, maxLegendSize.Height > lsiz.Height ? maxLegendSize.Height : lsiz.Height);

                    if (legend.LegendMargin > legendMargin)
                        legendMargin = legend.LegendMargin;
                }

                if (maxLegendSize.Width > 0 || maxLegendSize.Height > 0)
                {
                    plotArea = new OxyRect(plotArea.Left, plotArea.Top, plotArea.Width, Math.Max(0, plotArea.Height - (maxLegendSize.Height + legendMargin)));
                }

                foreach (LegendBase legend in this.Legends.Where(l => l.LegendPlacement == LegendPlacement.Inside && l.IsLegendVisible))
                {
                    double availableLegendWidth = plotArea.Width;
                    double availableLegendHeight = double.IsNaN(legend.LegendMaxHeight) ?
                        plotArea.Height : Math.Min(plotArea.Height, legend.LegendMaxHeight);

                    if (legend.LegendPlacement == LegendPlacement.Inside)
                    {
                        availableLegendWidth -= legend.LegendMargin * 2;
                        availableLegendHeight -= legend.LegendMargin * 2;
                    }

                    legend.LegendSize = legend.GetLegendSize(rc, new OxySize(availableLegendWidth, availableLegendHeight));
                }
            }

            if (plotArea.Height < 0)
            {
                plotArea = new OxyRect(plotArea.Left, plotArea.Top, plotArea.Width, 1);
            }

            if (plotArea.Width < 0)
            {
                plotArea = new OxyRect(plotArea.Left, plotArea.Top, 1, plotArea.Height);
            }

            this.PlotArea = plotArea;
            this.PlotAndAxisArea = plotArea.Inflate(this.ActualPlotMargins);

            switch (this.TitleHorizontalAlignment)
            {
                case TitleHorizontalAlignment.CenteredWithinView:
                    this.TitleArea = new OxyRect(
                        this.PlotBounds.Left,
                        this.PlotBounds.Top + this.Padding.Top,
                        this.Width,
                        titleSize.Height + (this.TitlePadding * 2));
                    break;
                default:
                    this.TitleArea = new OxyRect(
                        this.PlotArea.Left,
                        this.PlotBounds.Top + this.Padding.Top,
                        this.PlotArea.Width,
                        titleSize.Height + (this.TitlePadding * 2));
                    break;
            }

            foreach (LegendBase l in this.Legends)
            {
                l.LegendArea = l.GetLegendRectangle(l.LegendSize);
            }
        }
    }
}
