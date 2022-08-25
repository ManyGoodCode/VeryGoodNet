using System;

namespace SharpDX
{
    public abstract class DisposeBase : IDisposable
    {
        public event EventHandler<EventArgs> Disposing;
        public event EventHandler<EventArgs> Disposed;

        ~DisposeBase()
        {
            CheckAndDispose(false);
        }

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            CheckAndDispose(true);
        }

        private void CheckAndDispose(bool disposing)
        {
            if (!IsDisposed)
            {
	            EventHandler<EventArgs> disposingHandlers = Disposing;
	            if (disposingHandlers != null)
                    disposingHandlers(this, DisposeEventArgs.Get(disposing));

                Dispose(disposing);
                GC.SuppressFinalize(this);

                IsDisposed = true;

	            EventHandler<EventArgs> disposedHandlers = Disposed;
	            if (disposedHandlers != null)
                    disposedHandlers(this, DisposeEventArgs.Get(disposing));
            }
        }

        protected abstract void Dispose(bool disposing);
    }
}