
namespace OxyPlot
{
    public interface IViewCommand<in T> : IViewCommand where T : OxyInputEventArgs
    {
        void Execute(IView view, IController controller, T args);
    }
}
