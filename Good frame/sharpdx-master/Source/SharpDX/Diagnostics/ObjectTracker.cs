using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

using SharpDX.Collections;

namespace SharpDX.Diagnostics
{
    public class ComObjectEventArgs : EventArgs
    {
        public ComObject Object;
        public ComObjectEventArgs(ComObject o)
        {
            Object = o;
        }
    }


    public static class ObjectTracker
    {
        private static Dictionary<IntPtr, List<ObjectReference>> processGlobalObjectReferences;
        
        [ThreadStatic] 
        private static Dictionary<IntPtr, List<ObjectReference>> threadStaticObjectReferences;

        public static event EventHandler<ComObjectEventArgs> Tracked;

        public static event EventHandler<ComObjectEventArgs> UnTracked;

        public static Func<string> StackTraceProvider = GetStackTrace;

        private static Dictionary<IntPtr, List<ObjectReference>> ObjectReferences
        {
            get
            {
                Dictionary<IntPtr, List<ObjectReference>> objectReferences;
                if (Configuration.UseThreadStaticObjectTracking)
                {
                    if (threadStaticObjectReferences == null)
                        threadStaticObjectReferences = new Dictionary<IntPtr, List<ObjectReference>>(EqualityComparer.DefaultIntPtr);

                    objectReferences = threadStaticObjectReferences;
                }
                else
                {
                    if (processGlobalObjectReferences == null)
                        processGlobalObjectReferences = new Dictionary<IntPtr, List<ObjectReference>>(EqualityComparer.DefaultIntPtr);

                    objectReferences = processGlobalObjectReferences;
                }

                return objectReferences;
            }
        }

        public static string GetStackTrace()
        {
            try
            {
                throw new GetStackTraceException();
            }
            catch (GetStackTraceException ex)
            {
                return ex.StackTrace;
            }
        }

        public static void Track(ComObject comObject)
        {
            if (comObject == null || comObject.NativePointer == IntPtr.Zero)
                return;
            lock (ObjectReferences)
            {
                List<ObjectReference> referenceList;
                if (!ObjectReferences.TryGetValue(comObject.NativePointer, out referenceList))
                {
                    referenceList = new List<ObjectReference>();
                    ObjectReferences.Add(comObject.NativePointer, referenceList);
                }

                referenceList.Add(new ObjectReference(DateTime.Now, comObject, StackTraceProvider != null ? StackTraceProvider() : String.Empty));
                OnTracked(comObject);
            }
        }

        public static List<ObjectReference> Find(IntPtr comObjectPtr)
        {
            lock (ObjectReferences)
            {
                List<ObjectReference> referenceList;
                if (ObjectReferences.TryGetValue(comObjectPtr, out referenceList))
                    return new List<ObjectReference>(referenceList);
            }
            return new List<ObjectReference>();
        }

        public static ObjectReference Find(ComObject comObject)
        {
            lock (ObjectReferences)
            {
                List<ObjectReference> referenceList;
                if (ObjectReferences.TryGetValue(comObject.NativePointer, out referenceList))
                {
                    foreach (var objectReference in referenceList)
                    {
                        if (ReferenceEquals(objectReference.Object.Target, comObject))
                            return objectReference;
                    }
                }
            }            
            return null;
        }

        public static void UnTrack(ComObject comObject)
        {
            if (comObject == null || comObject.NativePointer == IntPtr.Zero)
                return;

            lock (ObjectReferences)
            {
                List<ObjectReference> referenceList;
                if (ObjectReferences.TryGetValue(comObject.NativePointer, out referenceList))
                {
                    for (int i = referenceList.Count-1; i >=0; i--)
                    {
                        var objectReference = referenceList[i];
                        if (ReferenceEquals(objectReference.Object.Target, comObject))
                            referenceList.RemoveAt(i);
                        else if (!objectReference.IsAlive)
                            referenceList.RemoveAt(i);
                    }

                    if (referenceList.Count == 0)
                        ObjectReferences.Remove(comObject.NativePointer);

                    OnUnTracked(comObject);
                }
            }
        }

        public static List<ObjectReference> FindActiveObjects()
        {
            var activeObjects = new List<ObjectReference>();
            lock (ObjectReferences)
            {
                foreach (var referenceList in ObjectReferences.Values)
                {
                    foreach (var objectReference in referenceList)
                    {
                        if (objectReference.IsAlive)
                            activeObjects.Add(objectReference);
                    }
                }
            }
            return activeObjects;
        }


        public static string ReportActiveObjects()
        {
            StringBuilder text = new StringBuilder();
            int count = 0;
            Dictionary<string, int> countPerType = new Dictionary<string, int>();

            foreach (ObjectReference findActiveObject in FindActiveObjects())
            {
                string findActiveObjectStr = findActiveObject.ToString();
                if (!string.IsNullOrEmpty(findActiveObjectStr))
                {
                    text.AppendFormat("[{0}]: {1}", count, findActiveObjectStr);

                    var target = findActiveObject.Object.Target;
                    if (target != null)
                    {
                        int typeCount;
                        string targetType = target.GetType().Name;
                        if (!countPerType.TryGetValue(targetType, out typeCount))
                        {
                            countPerType[targetType] = 0;
                        }
                        else
                            countPerType[targetType] = typeCount + 1;
                    }
                }
                count++;
            }

            List<string> keys = new List<string>(countPerType.Keys);
            keys.Sort();

            text.AppendLine();
            text.AppendLine("Count per Type:");
            foreach (string key in keys)
            {
                text.AppendFormat("{0} : {1}", key, countPerType[key]);
                text.AppendLine();
            }
            return text.ToString();
        }

        private static void OnTracked(ComObject obj)
        {
            EventHandler<ComObjectEventArgs> handler = Tracked;
            if (handler != null)
            {
                handler(null, new ComObjectEventArgs(obj));
            }
        }

        private static void OnUnTracked(ComObject obj)
        {
            EventHandler<ComObjectEventArgs> handler = UnTracked;
            if (handler != null)
            {
                handler(null, new ComObjectEventArgs(obj));
            }
        }

        private class GetStackTraceException : Exception
        {
        }
   }
}