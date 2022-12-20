
namespace OxyPlot
{
    using System;

    public class DelegateViewCommand<T> : IViewCommand<T>
        where T : OxyInputEventArgs
    {
        private readonly Action<IView, IController, T> handler;
        public DelegateViewCommand(Action<IView, IController, T> handler)
        {
            this.handler = handler;
        }

        public void Execute(IView view, IController controller, T args)
        {
            this.handler(view, controller, args);
        }

        public void Execute(IView view, IController controller, OxyInputEventArgs args)
        {
            this.handler(view, controller, (T)args);
        }
    }
}
