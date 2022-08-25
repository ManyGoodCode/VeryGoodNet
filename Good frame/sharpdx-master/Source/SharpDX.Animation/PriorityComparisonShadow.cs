using System;
using System.Runtime.InteropServices;

namespace SharpDX.Animation
{
    internal class PriorityComparisonShadow : SharpDX.ComObjectShadow
    {
        private static readonly PriorityComparisonVtbl Vtbl = new PriorityComparisonVtbl();
        public static IntPtr ToIntPtr(PriorityComparison callback)
        {
            return CppObject.ToCallbackPtr<PriorityComparison>(callback);
        }

        public class PriorityComparisonVtbl : ComObjectVtbl
        {
            public PriorityComparisonVtbl() : base(1)
            {
                AddMethod(new HasPriorityDelegate(HasPriorityImpl));
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate SharpDX.Result HasPriorityDelegate(IntPtr thisPtr, SharpDX.Animation.Storyboard scheduledStoryboard, SharpDX.Animation.Storyboard newStoryboard, SharpDX.Animation.PriorityEffect priorityEffect);
            private static SharpDX.Result HasPriorityImpl(IntPtr thisPtr, SharpDX.Animation.Storyboard scheduledStoryboard, SharpDX.Animation.Storyboard newStoryboard, SharpDX.Animation.PriorityEffect priorityEffect)
            {
                try
                {
                    var shadow = ToShadow<PriorityComparisonShadow>(thisPtr);
                    var callback = (PriorityComparison)shadow.Callback;
                    return callback.HasPriority(scheduledStoryboard, newStoryboard, priorityEffect) ? Result.Ok : Result.False;
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }
            }
        }

        protected override CppObjectVtbl GetVtbl
        {
            get { return Vtbl; }
        }
    }
}