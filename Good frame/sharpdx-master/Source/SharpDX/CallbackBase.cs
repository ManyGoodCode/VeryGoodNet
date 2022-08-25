using System;
using System.Threading;

namespace SharpDX
{
    /// <summary>
    /// Callback base implementation of <see cref="ICallbackable"/>.
    /// </summary>
    public abstract class CallbackBase : DisposeBase, ICallbackable
    {
        private int refCount = 1;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Release();
            }
        }
        
        public int AddReference()
        {
            int old = refCount;
            while (true)
            {
                if (old == 0)
                {
                    throw new ObjectDisposedException("Cannot add a reference to a nonreferenced item");
                }
                var current = Interlocked.CompareExchange(ref refCount, old + 1, old);
                if (current == old)
                {
                    return old + 1;
                }
                old = current;
            }
        }

        public int Release()
        {
            var old = refCount;
            while (true)
            {
                var current = Interlocked.CompareExchange(ref refCount, old - 1, old);

                if (current == old)
                {
                    if (old == 1)
                    {
                        var callback = ((ICallbackable)this);
                        callback.Shadow.Dispose();
                        callback.Shadow = null;
                    }
                    return old - 1;
                }
                old = current;
            }
        }

        public Result QueryInterface(ref Guid guid, out IntPtr comObject)
        {
            var container = (ShadowContainer)((ICallbackable)this).Shadow;
            comObject = container.Find(guid);
            if (comObject == IntPtr.Zero)
            {
                return Result.NoInterface;
            }
            return Result.Ok;
        }

        IDisposable ICallbackable.Shadow { get; set; }
    }
}