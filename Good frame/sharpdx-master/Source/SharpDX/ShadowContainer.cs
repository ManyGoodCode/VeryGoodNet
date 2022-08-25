using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;

namespace SharpDX
{
    internal class ShadowContainer : DisposeBase
    {
        private readonly Dictionary<Guid, CppObjectShadow> guidToShadow = new Dictionary<Guid, CppObjectShadow>();

        private static readonly Dictionary<Type, List<Type>> typeToShadowTypes = new Dictionary<Type, List<Type>>();

        private IntPtr guidPtr;
        public IntPtr[] Guids { get; private set; }

        public void Initialize(ICallbackable callbackable)
        {
            callbackable.Shadow = this;

            Type type = callbackable.GetType();
            List<Type> slimInterfaces;
            lock (typeToShadowTypes)
            {
                if (!typeToShadowTypes.TryGetValue(type, out slimInterfaces))
                {
                    List<Type> interfaces = type.GetTypeInfo().ImplementedInterfaces;
                    slimInterfaces = new List<Type>();
                    slimInterfaces.AddRange(interfaces);
                    typeToShadowTypes.Add(type, slimInterfaces);

                    foreach (Type item in interfaces)
                    {
                        ShadowAttribute shadowAttribute = ShadowAttribute.Get(item);
                        if (shadowAttribute == null)
                        {
                            slimInterfaces.Remove(item);
                            continue;
                        }

                        List<Type> inheritList = item.GetTypeInfo().ImplementedInterfaces;
                        foreach (Type inheritInterface in inheritList)
                        {
                            slimInterfaces.Remove(inheritInterface);
                        }
                    }
                }
            }

            CppObjectShadow iunknownShadow = null;
            foreach (Type item in slimInterfaces)
            {
                ShadowAttribute shadowAttribute = ShadowAttribute.Get(item);
                CppObjectShadow shadow = (CppObjectShadow)Activator.CreateInstance(shadowAttribute.Type);
                shadow.Initialize(callbackable);
                if (iunknownShadow == null)
                {
                    iunknownShadow = shadow;
                    guidToShadow.Add(ComObjectShadow.IID_IUnknown, iunknownShadow);
                }

                guidToShadow.Add(Utilities.GetGuidFromType(item), shadow);

                var inheritList = item.GetTypeInfo().ImplementedInterfaces;
                foreach (Type inheritInterface in inheritList)
                {
                    ShadowAttribute inheritShadowAttribute = ShadowAttribute.Get(inheritInterface);
                    if (inheritShadowAttribute == null)
                        continue;
                    guidToShadow.Add(Utilities.GetGuidFromType(inheritInterface), shadow);
                }
            }

            int countGuids = 0;
            foreach (var guidKey in guidToShadow.Keys)
            {
                if (guidKey != Utilities.GetGuidFromType(typeof(IInspectable)) && guidKey != Utilities.GetGuidFromType(typeof(IUnknown)))
                    countGuids++;
            }

            guidPtr = Marshal.AllocHGlobal(Utilities.SizeOf<Guid>() * countGuids);
            Guids = new IntPtr[countGuids];
            int i = 0;
            unsafe
            {
                var pGuid = (Guid*) guidPtr;
                foreach (var guidKey in guidToShadow.Keys)
                {
                    if (guidKey == Utilities.GetGuidFromType(typeof(IInspectable)) || guidKey == Utilities.GetGuidFromType(typeof(IUnknown)))
                        continue;

                    pGuid[i] = guidKey;
                    // Store the pointer
                    Guids[i] = new IntPtr(pGuid + i);
                    i++;
                }
            }
        }

        internal IntPtr Find(Type type)
        {
            return Find(Utilities.GetGuidFromType(type));
        }

        internal IntPtr Find(Guid guidType)
        {
            var shadow = FindShadow(guidType);
            return (shadow == null) ? IntPtr.Zero : shadow.NativePointer;
        }

        internal CppObjectShadow FindShadow(Guid guidType)
        {
            CppObjectShadow shadow;
            guidToShadow.TryGetValue(guidType, out shadow);
            return shadow;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var comObjectCallbackNative in guidToShadow.Values)
                    comObjectCallbackNative.Dispose();
                guidToShadow.Clear();

                if (guidPtr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(guidPtr);
                    guidPtr = IntPtr.Zero;
                }
            }
        }
    }
}