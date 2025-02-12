﻿namespace OxyPlot
{
    public abstract class ManipulatorBase<T> where T : OxyInputEventArgs
    {
        protected ManipulatorBase(IView view)
        {
            this.View = view;
        }

        public IView View { get; private set; }

        public virtual void Completed(T e)
        {
        }

        public virtual void Delta(T e)
        {
        }

        public virtual void Started(T e)
        {
        }
    }
}