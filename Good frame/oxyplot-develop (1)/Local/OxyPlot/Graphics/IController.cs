
namespace OxyPlot
{
    public interface IController
    {
        bool HandleMouseDown(IView view, OxyMouseDownEventArgs args);
        bool HandleMouseMove(IView view, OxyMouseEventArgs args);
        bool HandleMouseUp(IView view, OxyMouseEventArgs args);
        bool HandleMouseEnter(IView view, OxyMouseEventArgs args);
        bool HandleMouseLeave(IView view, OxyMouseEventArgs args);
        bool HandleMouseWheel(IView view, OxyMouseWheelEventArgs args);
        bool HandleTouchStarted(IView view, OxyTouchEventArgs args);
        bool HandleTouchDelta(IView view, OxyTouchEventArgs args);
        bool HandleTouchCompleted(IView view, OxyTouchEventArgs args);
        bool HandleKeyDown(IView view, OxyKeyEventArgs args);
        bool HandleGesture(IView view, OxyInputGesture gesture, OxyInputEventArgs args);
        void AddMouseManipulator(IView view, ManipulatorBase<OxyMouseEventArgs> manipulator, OxyMouseDownEventArgs args);
        void AddHoverManipulator(IView view, ManipulatorBase<OxyMouseEventArgs> manipulator, OxyMouseEventArgs args);
        void AddTouchManipulator(IView view, ManipulatorBase<OxyTouchEventArgs> manipulator, OxyTouchEventArgs args);
        void Bind(OxyMouseDownGesture gesture, IViewCommand<OxyMouseDownEventArgs> command);
        void Bind(OxyMouseEnterGesture gesture, IViewCommand<OxyMouseEventArgs> command);
        void Bind(OxyMouseWheelGesture gesture, IViewCommand<OxyMouseWheelEventArgs> command);
        void Bind(OxyTouchGesture gesture, IViewCommand<OxyTouchEventArgs> command);
        void Bind(OxyKeyGesture gesture, IViewCommand<OxyKeyEventArgs> command);
        void Unbind(OxyInputGesture gesture);
        void Unbind(IViewCommand command);
        void UnbindAll();
    }
}
