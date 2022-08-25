using System;
using System.Collections.Generic;

namespace SharpDX
{
    public class DisposeCollector : DisposeBase
    {
        private List<object> disposables;
        public int Count
        {
            get { return disposables.Count; }
        }

        public void DisposeAndClear(bool disposeManagedResources = true)
        {
            if (disposables == null)
                return;

            for (int i = disposables.Count - 1; i >= 0; i--)
            {
                var valueToDispose = disposables[i];
                if (valueToDispose is IDisposable disposable)
                {
                    if (disposeManagedResources)
                    {
                        disposable.Dispose();
                    }
                }
                else
                {
                    Utilities.FreeMemory((IntPtr)valueToDispose);
                }

                disposables.RemoveAt(i);
            }
            disposables.Clear();
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            DisposeAndClear(disposeManagedResources);
            disposables = null;
        }

        public T Collect<T>(T toDispose)
        {
            if (!(toDispose is IDisposable || toDispose is IntPtr))
                throw new ArgumentException("Argument must be IDisposable or IntPtr");

            if (toDispose is IntPtr)
            {
                var memoryPtr = (IntPtr)(object)toDispose;
                if (!Utilities.IsMemoryAligned(memoryPtr))
                    throw new ArgumentException("Memory pointer is invalid. Memory must have been allocated with Utilties.AllocateMemory");
            }

            if (!Equals(toDispose, default(T)))
            {
                if (disposables == null)
                    disposables = new List<object>();

                if (!disposables.Contains(toDispose))
                {
                    disposables.Add(toDispose);
                }
            }

            return toDispose;
        }

        public void RemoveAndDispose<T>(ref T objectToDispose)
        {
            if (disposables != null)
            {
                Remove(objectToDispose);

                var disposableObject = objectToDispose as IDisposable;
                if (disposableObject != null)
                {
                    disposableObject.Dispose();
                }
                else
                {
                    var localData = (object)objectToDispose;
                    var dataPointer = (IntPtr) localData;
                    Utilities.FreeMemory(dataPointer);
                }
                objectToDispose = default(T);
            }
        }

        public void Remove<T>(T toDisposeArg)
        {
            if (disposables != null && disposables.Contains(toDisposeArg))
            {
                disposables.Remove(toDisposeArg);
            }
        }
    }
}