using System;
using SharpDX.Mathematics.Interop;

namespace SharpDX.DXGI
{
    public partial class IVirtualSurfaceImageSourceNative 
    {
        private IVirtualSurfaceUpdatesCallbackNative callback;
        private EventHandler<EventArgs> updatesNeeded;

        public RawRectangle[] UpdateRectangles
        {
            get
            {
                int count = this.GetUpdateRectCount();
                var regionToUpdate = new RawRectangle[count];
                this.GetUpdateRects(regionToUpdate, count);
                return regionToUpdate;
            }
        }

        public event EventHandler<EventArgs> UpdatesNeeded
        {
            add
            {
                if (callback == null)
                {
                    callback = new VirtualSurfaceUpdatesCallbackNativeCallback(this);
                    RegisterForUpdatesNeeded(callback);
                }

                updatesNeeded = (EventHandler<EventArgs>)Delegate.Combine(updatesNeeded, value);

            }
            remove
            {

                updatesNeeded = (EventHandler<EventArgs>)Delegate.Remove(updatesNeeded, value);
            }
        }

        private void OnUpdatesNeeded()
        {
            if (updatesNeeded != null)
                updatesNeeded(this, EventArgs.Empty);
        }

        private class VirtualSurfaceUpdatesCallbackNativeCallback : CallbackBase, IVirtualSurfaceUpdatesCallbackNative
        {
            IVirtualSurfaceImageSourceNative eventCallback;

            public VirtualSurfaceUpdatesCallbackNativeCallback(IVirtualSurfaceImageSourceNative eventCallbackArg)
            {
                eventCallback = eventCallbackArg;
            }

            public void UpdatesNeeded()
            {
                eventCallback.OnUpdatesNeeded();
            }
        }
    }
}