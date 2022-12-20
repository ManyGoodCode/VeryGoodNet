
namespace OxyPlot
{
    public interface IView
    {
        Model ActualModel { get; }
        IController ActualController { get; }
        OxyRect ClientArea { get; }
        void SetCursorType(CursorType cursorType);
        void HideZoomRectangle();
        void ShowZoomRectangle(OxyRect rectangle);
    }
}
