namespace OxyPlot
{
    /// <summary>
    /// Specifies functionality for the plot views.
    /// </summary>
    public interface IPlotView : IView
    {
        /// <summary>
        /// Gets the actual  of the control.
        /// </summary>
        new PlotModel ActualModel { get; }

        /// <summary>
        /// Hides the tracker.
        /// </summary>
        void HideTracker();

        /// <summary>
        /// Invalidates the plot (not blocking the UI thread)
        /// </summary>
        void InvalidatePlot(bool updateData = true);

        /// <summary>
        /// 展示了跟踪器
        /// </summary>
        void ShowTracker(TrackerHitResult trackerHitResult);

        /// <summary>
        /// 将文本存储在剪贴板上
        /// </summary>
        void SetClipboardText(string text);
    }
}