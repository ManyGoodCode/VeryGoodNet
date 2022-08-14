namespace OxyPlot
{
    public static class PlotCommands
    {
        static PlotCommands()
        {
            // commands that can be triggered from key events
            Reset = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandleReset(view, args));
            CopyCode = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandleCopyCode(view, args));

            // commands that can be triggered from mouse down events
            ResetAt = new DelegatePlotCommand<OxyMouseEventArgs>((view, controller, args) => HandleReset(view, args));
            PanAt = new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new PanManipulator(view), args));
            ZoomRectangle = new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new ZoomRectangleManipulator(view), args));
            Track = new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new TrackerManipulator(view) { Snap = false, PointsOnly = false }, args));
            SnapTrack = new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new TrackerManipulator(view) { Snap = true, PointsOnly = false }, args));
            PointsOnlyTrack = new DelegatePlotCommand<OxyMouseDownEventArgs>((view, controller, args) => controller.AddMouseManipulator(view, new TrackerManipulator(view) { Snap = false, PointsOnly = true }, args));
            ZoomWheel = new DelegatePlotCommand<OxyMouseWheelEventArgs>((view, controller, args) => HandleZoomByWheel(view, args));
            ZoomWheelFine = new DelegatePlotCommand<OxyMouseWheelEventArgs>((view, controller, args) => HandleZoomByWheel(view, args, 0.1));
            ZoomInAt = new DelegatePlotCommand<OxyMouseEventArgs>((view, controller, args) => HandleZoomAt(view, args, 0.05));
            ZoomOutAt = new DelegatePlotCommand<OxyMouseEventArgs>((view, controller, args) => HandleZoomAt(view, args, -0.05));

            // commands that can be triggered from mouse enter events
            HoverTrack = new DelegatePlotCommand<OxyMouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new TrackerManipulator(view) { LockToInitialSeries = false, Snap = false, PointsOnly = false }, args));
            HoverSnapTrack = new DelegatePlotCommand<OxyMouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new TrackerManipulator(view) { LockToInitialSeries = false, Snap = true, PointsOnly = false }, args));
            HoverPointsOnlyTrack = new DelegatePlotCommand<OxyMouseEventArgs>((view, controller, args) => controller.AddHoverManipulator(view, new TrackerManipulator(view) { LockToInitialSeries = false, Snap = false, PointsOnly = true }, args));

            // Touch events
            SnapTrackTouch = new DelegatePlotCommand<OxyTouchEventArgs>((view, controller, args) => controller.AddTouchManipulator(view, new TouchTrackerManipulator(view) { Snap = true, PointsOnly = false }, args));
            PointsOnlyTrackTouch = new DelegatePlotCommand<OxyTouchEventArgs>((view, controller, args) => controller.AddTouchManipulator(view, new TouchTrackerManipulator(view) { Snap = true, PointsOnly = true }, args));
            PanZoomByTouch = new DelegatePlotCommand<OxyTouchEventArgs>((view, controller, args) => controller.AddTouchManipulator(view, new TouchManipulator(view), args));

            // commands that can be triggered from key events
            PanLeft = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandlePan(view, args, -0.1, 0));
            PanRight = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0.1, 0));
            PanUp = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0, -0.1));
            PanDown = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0, 0.1));
            PanLeftFine = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandlePan(view, args, -0.01, 0));
            PanRightFine = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0.01, 0));
            PanUpFine = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0, -0.01));
            PanDownFine = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandlePan(view, args, 0, 0.01));

            ZoomIn = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandleZoomCenter(view, args, 1));
            ZoomOut = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandleZoomCenter(view, args, -1));
            ZoomInFine = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandleZoomCenter(view, args, 0.1));
            ZoomOutFine = new DelegatePlotCommand<OxyKeyEventArgs>((view, controller, args) => HandleZoomCenter(view, args, -0.1));
        }

        public static IViewCommand<OxyKeyEventArgs> Reset { get; private set; }

        public static IViewCommand<OxyMouseEventArgs> ResetAt { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> CopyCode { get; private set; }

        public static IViewCommand<OxyTouchEventArgs> PanZoomByTouch { get; private set; }

        public static IViewCommand<OxyMouseDownEventArgs> PanAt { get; private set; }

        public static IViewCommand<OxyMouseDownEventArgs> ZoomRectangle { get; private set; }

        public static IViewCommand<OxyMouseWheelEventArgs> ZoomWheel { get; private set; }

        public static IViewCommand<OxyMouseWheelEventArgs> ZoomWheelFine { get; private set; }

        public static IViewCommand<OxyMouseDownEventArgs> Track { get; private set; }

        public static IViewCommand<OxyMouseDownEventArgs> SnapTrack { get; private set; }

        public static IViewCommand<OxyTouchEventArgs> SnapTrackTouch { get; private set; }

        public static IViewCommand<OxyMouseDownEventArgs> PointsOnlyTrack { get; private set; }

        public static IViewCommand<OxyTouchEventArgs> PointsOnlyTrackTouch { get; private set; }

        public static IViewCommand<OxyMouseEventArgs> HoverTrack { get; private set; }

        public static IViewCommand<OxyMouseEventArgs> HoverSnapTrack { get; private set; }

        public static IViewCommand<OxyMouseEventArgs> HoverPointsOnlyTrack { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> PanLeft { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> PanRight { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> PanUp { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> PanDown { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> PanLeftFine { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> PanRightFine { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> PanUpFine { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> PanDownFine { get; private set; }

        public static IViewCommand<OxyMouseEventArgs> ZoomInAt { get; private set; }

        public static IViewCommand<OxyMouseEventArgs> ZoomOutAt { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> ZoomIn { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> ZoomOut { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> ZoomInFine { get; private set; }

        public static IViewCommand<OxyKeyEventArgs> ZoomOutFine { get; private set; }

        private static void HandleReset(IPlotView view, OxyInputEventArgs args)
        {
            args.Handled = true;
            view.ActualModel.ResetAllAxes();
            view.InvalidatePlot(false);
        }

        private static void HandleCopyCode(IPlotView view, OxyInputEventArgs args)
        {
            args.Handled = true;
            string code = view.ActualModel.ToCode();
            view.SetClipboardText(code);
        }

        private static void HandleZoomAt(IPlotView view, OxyMouseEventArgs args, double delta)
        {
            ZoomStepManipulator m = new ZoomStepManipulator(view) { Step = delta, FineControl = args.IsControlDown };
            m.Started(args);
        }


        private static void HandleZoomByWheel(IPlotView view, OxyMouseWheelEventArgs args, double factor = 1)
        {
            ZoomStepManipulator m = new ZoomStepManipulator(view) { Step = args.Delta * 0.001 * factor, FineControl = args.IsControlDown };
            m.Started(args);
        }

        private static void HandleZoomCenter(IPlotView view, OxyInputEventArgs args, double delta)
        {
            args.Handled = true;
            view.ActualModel.ZoomAllAxes(1 + (delta * 0.12));
            view.InvalidatePlot(false);
        }

        private static void HandlePan(IPlotView view, OxyInputEventArgs args, double dx, double dy)
        {
            args.Handled = true;
            dx *= view.ActualModel.PlotArea.Width;
            dy *= view.ActualModel.PlotArea.Height;
            view.ActualModel.PanAllAxes(dx, dy);
            view.InvalidatePlot(false);
        }
    }
}
