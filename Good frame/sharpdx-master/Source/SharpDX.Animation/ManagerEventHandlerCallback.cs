using System;
using System.Runtime.InteropServices;

namespace SharpDX.Animation
{
    internal class ManagerEventHandlerCallback : SharpDX.CallbackBase, SharpDX.ICallbackable, ManagerEventHandler
    {
        public Manager.StatusChangedDelegate Delegates;
        public static IntPtr ToIntPtr(ManagerEventHandler callback)
        {
            return CppObject.ToCallbackPtr<ManagerEventHandler>(callback);
        }

        public void OnManagerStatusChanged(ManagerStatus newStatus, ManagerStatus previousStatus)
        {
            if (Delegates != null)
                Delegates(newStatus, previousStatus);
        }
    }
}