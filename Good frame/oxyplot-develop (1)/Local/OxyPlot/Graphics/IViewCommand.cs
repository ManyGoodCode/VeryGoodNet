
namespace OxyPlot
{
    public interface IViewCommand
    {
        void Execute(IView view, IController controller, OxyInputEventArgs args);
    }
}
