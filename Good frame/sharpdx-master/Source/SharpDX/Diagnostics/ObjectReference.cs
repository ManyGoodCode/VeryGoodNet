using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace SharpDX.Diagnostics
{
    public class ObjectReference
    {
        public ObjectReference(DateTime creationTime, ComObject comObject, string stackTrace)
        {
            CreationTime = creationTime;
            Object = new WeakReference(comObject, true);
            StackTrace = stackTrace;
        }

        public DateTime CreationTime { get; private set; }
        public WeakReference Object { get; private set; }
        public string StackTrace { get; private set; }

        public bool IsAlive
        {
            get { return Object.IsAlive; }
        }
        public override string ToString()
        {
            ComObject comObject = Object.Target as ComObject;
            if (comObject == null)
                return "";

            StringBuilder builder = new StringBuilder();

            builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "Active COM Object: [0x{0:X}] Class: [{1}] Time [{2}] Stack:\r\n{3}",
                comObject.NativePointer.ToInt64(),
                comObject.GetType().FullName,
                CreationTime,
                StackTrace).AppendLine();

            return builder.ToString();
        }
    }

}