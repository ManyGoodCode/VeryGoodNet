using System;
using System.Runtime.InteropServices;

namespace SharpDX
{
    internal class InspectableShadow : ComObjectShadow
    {
        private static readonly InspectableProviderVtbl Vtbl = new InspectableProviderVtbl();
        public static IntPtr ToIntPtr(IInspectable callback)
        {
            return ToCallbackPtr<IInspectable>(callback);
        }

        public class InspectableProviderVtbl : ComObjectVtbl
        {
            public InspectableProviderVtbl()
                : base(3)
            {
                unsafe
                {
                    AddMethod(new GetIidsDelegate(GetIids));
                    AddMethod(new GetRuntimeClassNameDelegate(GetRuntimeClassName));
                    AddMethod(new GetTrustLevelDelegate(GetTrustLevel));
                }
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private unsafe delegate int GetIidsDelegate(IntPtr thisPtr, int* iidCount, IntPtr* iids);
            private unsafe static int GetIids(IntPtr thisPtr, int* iidCount, IntPtr* iids)
            {
                try
                {
                    InspectableShadow shadow = ToShadow<InspectableShadow>(thisPtr);
                    IInspectable callback = (IInspectable)shadow.Callback;

                    ShadowContainer container = (ShadowContainer) callback.Shadow;

                    int countGuids = container.Guids.Length;

                    iids = (IntPtr*)Marshal.AllocCoTaskMem(IntPtr.Size * countGuids);
                    *iidCount = countGuids;

                    for (int i = 0; i < countGuids; i++)
                        iids[i] = container.Guids[i];
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }

                return Result.Ok.Code;
            }

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int GetRuntimeClassNameDelegate(IntPtr thisPtr, IntPtr className);
            private static int GetRuntimeClassName(IntPtr thisPtr, IntPtr className)
            {
                try
                {
                    InspectableShadow shadow = ToShadow<InspectableShadow>(thisPtr);
                    IInspectable callback = (IInspectable)shadow.Callback;
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }

                return Result.Ok.Code;
            }

            enum TrustLevel
            {
                BaseTrust = 0,
                PartialTrust = (BaseTrust + 1),
                FullTrust = (PartialTrust + 1)
            };

            [UnmanagedFunctionPointer(CallingConvention.StdCall)]
            private delegate int GetTrustLevelDelegate(IntPtr thisPtr, IntPtr trustLevel);
            private static int GetTrustLevel(IntPtr thisPtr, IntPtr trustLevel)
            {
                try
                {
                    InspectableShadow shadow = ToShadow<InspectableShadow>(thisPtr);
                    IInspectable callback = (IInspectable)shadow.Callback;
                    Marshal.WriteInt32(trustLevel, (int)TrustLevel.FullTrust);
                }
                catch (Exception exception)
                {
                    return (int)SharpDX.Result.GetResultFromException(exception);
                }

                return Result.Ok.Code;
            }

        }

        protected override CppObjectVtbl GetVtbl
        {
            get { return Vtbl; }
        }
    }
}