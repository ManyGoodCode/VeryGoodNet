using System;
using System.Runtime.InteropServices;

namespace SharpDX.Animation
{
    public partial class Variable
    {
        public Variable(Manager manager, double initialValue = 0.0)
        {
            manager.CreateAnimationVariable(initialValue, this);
        }

        public void SetTag(object @object, int id)
        {
            IntPtr tagObjectPtr = IntPtr.Zero;
            int previousId;
            GetTag(out tagObjectPtr, out previousId);
            if (tagObjectPtr != IntPtr.Zero)
                GCHandle.FromIntPtr(tagObjectPtr).Free();

            SetTag(GCHandle.ToIntPtr(GCHandle.Alloc(@object)), id);
        }

        public void GetTag(out object @object, out int id)
        {
            IntPtr tagObjectPtr;
            GetTag(out tagObjectPtr, out id);
            @object = GCHandle.FromIntPtr(tagObjectPtr).Target;
        }
    }
}