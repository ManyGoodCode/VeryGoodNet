using System;

namespace SharpDX
{
    public class DisposeEventArgs : EventArgs
    {
        public static readonly DisposeEventArgs DisposingEventArgs = new DisposeEventArgs(true);
        public static readonly DisposeEventArgs NotDisposingEventArgs = new DisposeEventArgs(false);
        public readonly bool Disposing;

        private DisposeEventArgs(bool disposing)
        {
            Disposing = disposing;
        }

        public static DisposeEventArgs Get(bool disposing)
        {
            return disposing ? DisposingEventArgs : NotDisposingEventArgs;
        }
    }
}
