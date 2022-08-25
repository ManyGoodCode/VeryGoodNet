using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;

using SharpDX.Direct3D;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using SharpDX.Text;

using SharpDX.Mathematics.Interop;

namespace SharpDX
{
    public delegate void GetValueFastDelegate<T>(object obj, out T value);
    public delegate void SetValueFastDelegate<T>(object obj, ref T value);

    public static class Utilities
    {
        public static void CopyMemory(IntPtr dest, IntPtr src, int sizeInBytesToCopy)
        {
            unsafe
            {
                Interop.memcpy((void*)dest, (void*)src, sizeInBytesToCopy);
            }
        }

        public unsafe static bool CompareMemory(IntPtr from, IntPtr against, int sizeToCompare)
        {
            var pSrc = (byte*)@from;
            var pDst = (byte*)against;

            int numberOf = sizeToCompare >> 3;
            while (numberOf > 0)
            {
                if (*(long*)pSrc != *(long*)pDst)
                    return false;
                pSrc += 8;
                pDst += 8;
                numberOf--;
            }
 
            numberOf = sizeToCompare & 7;
            while (numberOf > 0)
            {
                if (*pSrc != *pDst)
                    return false;
                pSrc++;
                pDst++;
                numberOf--;
            }

            return true;
        }

        public static void ClearMemory(IntPtr dest, byte value, int sizeInBytesToClear)
        {
            unsafe
            {
                Interop.memset((void*)dest, value, sizeInBytesToClear);
            }
        }

        public static int SizeOf<T>() where T : struct
        {
            return Interop.SizeOf<T>();            
        }

        public static int SizeOf<T>(T[] array) where T : struct
        {
            return array == null ? 0 : array.Length * Interop.SizeOf<T>();
        }

        public static void Pin<T>(ref T source, Action<IntPtr> pinAction) where T : struct
        {
            unsafe
            {
                pinAction((IntPtr)Interop.Fixed(ref source));
            }
        }

        public static void Pin<T>(T[] source, Action<IntPtr> pinAction) where T : struct
        {
            unsafe
            {
                pinAction(source == null ? IntPtr.Zero : (IntPtr)Interop.Fixed(source));
            }
        }

        public static byte[] ToByteArray<T>(T[] source) where T : struct
        {
            if (source == null) 
                return null;
            var buffer = new byte[SizeOf<T>() * source.Length];
            if (source.Length == 0)
                return buffer;
            unsafe
            {
                fixed (void* pBuffer = buffer)
                    Interop.Write(pBuffer, source, 0, source.Length);
            }

            return buffer;
        }

        public static void Swap<T>(ref T left, ref T right)
        {
            var temp = left;
            left = right;
            right = temp;
        }

        public static T Read<T>(IntPtr source) where T : struct
        {
            unsafe
            {
                return Interop.ReadInline<T>((void*)source);
            }
        }

        public static void Read<T>(IntPtr source, ref T data) where T : struct
        {
            unsafe
            {
                Interop.CopyInline(ref data, (void*)source);
            }
        }

        public static void ReadOut<T>(IntPtr source, out T data) where T : struct
        {
            unsafe
            {
                Interop.CopyInlineOut(out data, (void*)source);
            }
        }

        public static IntPtr ReadAndPosition<T>(IntPtr source, ref T data) where T : struct
        {
            unsafe
            {
                return (IntPtr)Interop.Read((void*)source, ref data);
            }
        }

        public static IntPtr Read<T>(IntPtr source, T[] data, int offset, int count) where T : struct
        {
            unsafe
            {
                return (IntPtr)Interop.Read((void*)source, data, offset, count);
            }
        }

        public static void Write<T>(IntPtr destination, ref T data) where T : struct
        {
            unsafe
            {
                Interop.CopyInline((void*)destination, ref data);
            }
        }

        public static IntPtr WriteAndPosition<T>(IntPtr destination, ref T data) where T : struct
        {
            unsafe
            {
                return (IntPtr)Interop.Write((void*)destination, ref data);
            }
        }

        public static IntPtr Write<T>(IntPtr destination, T[] data, int offset, int count) where T : struct
        {
            unsafe
            {
                return (IntPtr)Interop.Write((void*)destination, data, offset, count);
            }
        }

        public unsafe static void ConvertToIntArray(bool[] array, int* dest)
        {
            for (int i = 0; i < array.Length; i++)
                dest[i] = array[i] ? 1 : 0;
        }
 
        public static RawBool[] ConvertToIntArray(bool[] array)
        {
            var temp = new RawBool[array.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = array[i];
            return temp;
        }

        public static unsafe bool[] ConvertToBoolArray(int* array, int length)
        {
            var temp = new bool[length];
            for(int i = 0; i < temp.Length; i++)
                temp[i] = array[i] != 0;
            return temp;
        }

        public static bool[] ConvertToBoolArray(RawBool[] array)
        {
            var temp = new bool[array.Length];
            for(int i = 0; i < temp.Length; i++)
                temp[i] = array[i];
            return temp;
        }

        public static Guid GetGuidFromType(Type type)
        {
            return type.GetTypeInfo().GUID;
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetTypeInfo().ImplementedInterfaces;
            foreach (var it in interfaceTypes)
            {
                if (it.GetTypeInfo().IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.GetTypeInfo().IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.GetTypeInfo().BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public unsafe static IntPtr AllocateMemory(int sizeInBytes, int align = 16)
        {
            int mask = align - 1;
            var memPtr = Marshal.AllocHGlobal(sizeInBytes + mask + IntPtr.Size);
            var ptr = (long)((byte*)memPtr + sizeof(void*) + mask) & ~mask;
            ((IntPtr*)ptr)[-1] = memPtr;
            return new IntPtr((void*)ptr);
        }

        public static IntPtr AllocateClearedMemory(int sizeInBytes, byte clearValue = 0, int align = 16)
        {
            var ptr = AllocateMemory(sizeInBytes, align);
            ClearMemory(ptr, clearValue, sizeInBytes);
            return ptr;
        }

        public static bool IsMemoryAligned(IntPtr memoryPtr, int align = 16)
        {
            return ((memoryPtr.ToInt64() & (align-1)) == 0);
        }

        public unsafe static void FreeMemory(IntPtr alignedBuffer)
        {
            if (alignedBuffer == IntPtr.Zero) return;
            Marshal.FreeHGlobal(((IntPtr*) alignedBuffer)[-1]);
        }

        public static string PtrToStringAnsi(IntPtr pointer, int maxLength)
        {
            string managedString = Marshal.PtrToStringAnsi(pointer); // copy null-terminating unmanaged text from pointer to a managed string
            if (managedString != null && managedString.Length > maxLength)
                managedString = managedString.Substring(0, maxLength);

            return managedString;
        }

        public static string PtrToStringUni(IntPtr pointer, int maxLength)
        {
            string managedString = Marshal.PtrToStringUni(pointer); // copy null-terminating unmanaged text from pointer to a managed string
            if (managedString != null && managedString.Length > maxLength)
                managedString = managedString.Substring(0, maxLength);

            return managedString;
        }

        public static unsafe IntPtr StringToHGlobalAnsi(string s)
        {
            return Marshal.StringToHGlobalAnsi(s);
        }

        public static unsafe IntPtr StringToHGlobalUni(string s)
        {
            return Marshal.StringToHGlobalUni(s);
        }

        public static unsafe IntPtr StringToCoTaskMemUni(string s)
        {
            if (s == null)
            {
                return IntPtr.Zero;
            }
            int num = (s.Length + 1) * 2;
            if (num < s.Length)
            {
                throw new ArgumentOutOfRangeException("s");
            }
            IntPtr ptr2 = Marshal.AllocCoTaskMem(num);
            if (ptr2 == IntPtr.Zero)
            {
                throw new OutOfMemoryException();
            }
            CopyStringToUnmanaged(ptr2, s);
            return ptr2;
        }

        private unsafe static void CopyStringToUnmanaged(IntPtr ptr, string str)
        {
            fixed (char* pStr = str)
            {
                CopyMemory(ptr, new IntPtr(pStr), (str.Length + 1 ) * 2);
            }
        }

        public static IntPtr GetIUnknownForObject(object obj)
        {
            IntPtr objPtr =  obj == null ? IntPtr.Zero : Marshal.GetIUnknownForObject(obj);
            return objPtr;
        }

        public static object GetObjectForIUnknown(IntPtr iunknownPtr)
        {
            return iunknownPtr == IntPtr.Zero ? null : Marshal.GetObjectForIUnknown(iunknownPtr);
        }

        public static string Join<T>(string separator, T[] array)
        {
            var text = new StringBuilder();
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (i > 0) text.Append(separator);
                    text.Append(array[i]);
                }
            }
            return text.ToString();
        }

        public static string Join(string separator, IEnumerable elements)
        {
            var elementList = new List<string>();
            foreach (var element in elements)
                elementList.Add(element.ToString());

            var text = new StringBuilder();
            for (int i = 0; i < elementList.Count; i++)
            {
                var element = elementList[i];
                if (i > 0) text.Append(separator);
                text.Append(element);
            }
            return text.ToString();
        }

        public static string Join(string separator, IEnumerator elements)
        {
            var elementList = new List<string>();
            while (elements.MoveNext())
                elementList.Add(elements.Current.ToString());

            var text = new StringBuilder();
            for (int i = 0; i < elementList.Count; i++)
            {
                var element = elementList[i];
                if (i > 0) text.Append(separator);
                text.Append(element);
            }
            return text.ToString();
        }

        public static string BlobToString(Blob blob)
        {
            if (blob == null) return null;
            string output;
            output = Marshal.PtrToStringAnsi(blob.BufferPointer);
            blob.Dispose();
            return output;
        }

        public unsafe static IntPtr IntPtrAdd(IntPtr ptr, int offset)
        {
            return new IntPtr(((byte*) ptr) + offset);
        }

        public static byte[] ReadStream(Stream stream)
        {
            int readLength = 0;
            return ReadStream(stream, ref readLength);
        }

        public static byte[] ReadStream(Stream stream, ref int readLength)
        {
            Debug.Assert(stream != null);
            Debug.Assert(stream.CanRead);
            int num = readLength;
            Debug.Assert(num <= (stream.Length - stream.Position));
            if (num == 0)
                readLength = (int) (stream.Length - stream.Position);
            num = readLength;

            Debug.Assert(num >= 0);
            if (num == 0)
                return new byte[0];

            byte[] buffer = new byte[num];
            int bytesRead = 0;
            if (num > 0)
            {
                do
                {
                    bytesRead += stream.Read(buffer, bytesRead, readLength - bytesRead);
                } while (bytesRead < readLength);
            }
            return buffer;
        }

        public static bool Compare(IEnumerable left, IEnumerable right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return Compare(left.GetEnumerator(), right.GetEnumerator());
        }

        public static bool Compare(IEnumerator leftIt, IEnumerator rightIt)
        {
            if (ReferenceEquals(leftIt, rightIt))
                return true;
            if (ReferenceEquals(leftIt, null) || ReferenceEquals(rightIt, null))
                return false;

            bool hasLeftNext;
            bool hasRightNext;
            while (true)
            {
                hasLeftNext = leftIt.MoveNext();
                hasRightNext = rightIt.MoveNext();
                if (!hasLeftNext || !hasRightNext)
                    break;

                if (!Equals(leftIt.Current, rightIt.Current))
                    return false;
            }

            // If there is any left element
            if (hasLeftNext != hasRightNext)
                return false;

            return true;
        }

        public static bool Compare(ICollection left, ICollection right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            if (left.Count != right.Count)
                return false;

            int count = 0;
            var leftIt = left.GetEnumerator();
            var rightIt = right.GetEnumerator();
            while (leftIt.MoveNext() && rightIt.MoveNext())
            {
                if (!Equals(leftIt.Current, rightIt.Current))
                    return false;
                count++;
            }

            if (count != left.Count)
                return false;

            return true;
        }

        public static T GetCustomAttribute<T>(MemberInfo memberInfo, bool inherited = false) where T : Attribute
        {
            return memberInfo.GetCustomAttribute<T>(inherited);
        }

        public static IEnumerable<T> GetCustomAttributes<T>(MemberInfo memberInfo, bool inherited = false) where T : Attribute
        {
            return memberInfo.GetCustomAttributes<T>(inherited);
        }

        public static bool IsAssignableFrom(Type toType, Type fromType)
        {
            return toType.GetTypeInfo().IsAssignableFrom(fromType.GetTypeInfo());
        }

        public static bool IsEnum(Type typeToTest)
        {
            return typeToTest.GetTypeInfo().IsEnum;
        }

        public static bool IsValueType(Type typeToTest)
        {
            return typeToTest.GetTypeInfo().IsValueType;
        }

        private static MethodInfo GetMethod(Type type, string name, Type[] typeArgs) {

            foreach( var method in type.GetTypeInfo().GetDeclaredMethods(name))
            {
                if ( method.GetParameters().Length == typeArgs.Length) {
                    var parameters = method.GetParameters();
                    bool methodFound = true;
                    for (int i = 0; i < typeArgs.Length; i++)
                    {
                        if (parameters[i].ParameterType != typeArgs[i]) {
                            methodFound = false;
                            break;
                        }
                    }
                    if (methodFound) {
                        return method;
                    }
                }
            }
            return null;
        }

        public static GetValueFastDelegate<T> BuildPropertyGetter<T>(Type customEffectType, PropertyInfo propertyInfo)
        {
            var valueParam = Expression.Parameter(typeof(T).MakeByRefType());
            var objectParam = Expression.Parameter(typeof(object));
            var castParam = Expression.Convert(objectParam, customEffectType);
            var propertyAccessor = Expression.Property(castParam, propertyInfo);

            Expression convertExpression;
            if (propertyInfo.PropertyType == typeof(bool))
            {
                convertExpression = Expression.Condition(propertyAccessor, Expression.Constant(1), Expression.Constant(0));
            }
            else
            {
                convertExpression = Expression.Convert(propertyAccessor, typeof(T));
            }
            return Expression.Lambda<GetValueFastDelegate<T>>(Expression.Assign(valueParam, convertExpression), objectParam, valueParam).Compile();
        }

        public static SetValueFastDelegate<T> BuildPropertySetter<T>(Type customEffectType, PropertyInfo propertyInfo)
        {
            var valueParam = Expression.Parameter(typeof(T).MakeByRefType());
            var objectParam = Expression.Parameter(typeof(object));
            var castParam = Expression.Convert(objectParam, customEffectType);
            var propertyAccessor = Expression.Property(castParam, propertyInfo);

            Expression convertExpression;
            if (propertyInfo.PropertyType == typeof(bool))
            {
                convertExpression = Expression.NotEqual(valueParam, Expression.Constant(0));
            }
            else
            {
                convertExpression = Expression.Convert(valueParam, propertyInfo.PropertyType);
            }
            return Expression.Lambda<SetValueFastDelegate<T>>(Expression.Assign(propertyAccessor, convertExpression), objectParam, valueParam).Compile();
        }

        private static MethodInfo FindExplicitConverstion(Type sourceType, Type targetType)
        {
            if (sourceType == targetType)
                return null;

            var methods = new List<MethodInfo>();

            var tempType = sourceType;
            while (tempType != null)
            {
                methods.AddRange(tempType.GetTypeInfo().DeclaredMethods); //target methods will be favored in the search
                tempType = tempType.GetTypeInfo().BaseType;
            }

            tempType = targetType;
            while (tempType != null)
            {
                methods.AddRange(tempType.GetTypeInfo().DeclaredMethods); //target methods will be favored in the search
                tempType = tempType.GetTypeInfo().BaseType;
            }

            foreach (MethodInfo mi in methods)
            {
                if (mi.Name == "op_Explicit") //will return target and take one parameter
                    if (mi.ReturnType == targetType)
                        if (IsAssignableFrom(mi.GetParameters()[0].ParameterType, sourceType))
                            return mi;
            }

            return null;
        }

        [Flags]
        public enum CLSCTX : uint
        {
            ClsctxInprocServer = 0x1,
            ClsctxInprocHandler = 0x2,
            ClsctxLocalServer = 0x4,
            ClsctxInprocServer16 = 0x8,
            ClsctxRemoteServer = 0x10,
            ClsctxInprocHandler16 = 0x20,
            ClsctxReserved1 = 0x40,
            ClsctxReserved2 = 0x80,
            ClsctxReserved3 = 0x100,
            ClsctxReserved4 = 0x200,
            ClsctxNoCodeDownload = 0x400,
            ClsctxReserved5 = 0x800,
            ClsctxNoCustomMarshal = 0x1000,
            ClsctxEnableCodeDownload = 0x2000,
            ClsctxNoFailureLog = 0x4000,
            ClsctxDisableAaa = 0x8000,
            ClsctxEnableAaa = 0x10000,
            ClsctxFromDefaultContext = 0x20000,
            ClsctxInproc = ClsctxInprocServer | ClsctxInprocHandler,
            ClsctxServer = ClsctxInprocServer | ClsctxLocalServer | ClsctxRemoteServer,
            ClsctxAll = ClsctxServer | ClsctxInprocHandler
        }

        [DllImport("ole32.dll", ExactSpelling = true, EntryPoint = "CoCreateInstance", PreserveSig = true)]
        private static extern Result CoCreateInstance([In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, IntPtr pUnkOuter, CLSCTX dwClsContext, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr comObject);

        internal static void CreateComInstance(Guid clsid, CLSCTX clsctx, Guid riid, ComObject comObject)
        {
            IntPtr pointer;
            var result = CoCreateInstance(clsid, IntPtr.Zero, clsctx, riid, out pointer);
            result.CheckError();
            comObject.NativePointer = pointer;
        }

        internal static bool TryCreateComInstance(Guid clsid, CLSCTX clsctx, Guid riid, ComObject comObject)
        {
            IntPtr pointer;
            var result = CoCreateInstance(clsid, IntPtr.Zero, clsctx, riid, out pointer);
            comObject.NativePointer = pointer;
            return result.Success;
        }

        public enum CoInit
        {
            MultiThreaded = 0x0,
            ApartmentThreaded = 0x2,
            DisableOle1Dde = 0x4,
            SpeedOverMemory = 0x8
        }

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr handle);

        public static IntPtr GetProcAddress(IntPtr handle, string dllFunctionToImport)
        {
            IntPtr result = GetProcAddress_(handle, dllFunctionToImport);
            if (result == IntPtr.Zero)
                throw new SharpDXException(dllFunctionToImport);
            return result;
        }

        [DllImport("kernel32", EntryPoint = "GetProcAddress", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress_(IntPtr hModule, string procName);

        public static int ComputeHashFNVModified(byte[] data)
        {
            const uint p = 16777619;
            uint hash = 2166136261;
            foreach (byte b in data)
                hash = (hash ^ b) * p;
            hash += hash << 13;
            hash ^= hash >> 7;
            hash += hash << 3;
            hash ^= hash >> 17;
            hash += hash << 5;
            return unchecked((int)hash);
        }

        public static void Dispose<T>(ref T comObject) where T : class, IDisposable
        {
            if (comObject != null)
            {
                comObject.Dispose();
                comObject = null;
            }
        }

        public static T[] ToArray<T>(IEnumerable<T> source)
        {
            return new Buffer<T>(source).ToArray();
        }

        public static bool Any<T>(IEnumerable<T> source)
        {
            return source.GetEnumerator().MoveNext();
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            foreach (TSource sourceItem in source)
            {
                foreach (TResult result in selector(sourceItem))
                    yield return result;
            }
        }

        public static IEnumerable<TSource> Distinct<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer = null)
        {
            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;
            
            var values = new Dictionary<TSource, object>(comparer);
            foreach (TSource sourceItem in source)
            {
                if (!values.ContainsKey(sourceItem))
                {
                    values.Add(sourceItem, null);
                    yield return sourceItem;
                }
            }
        }

        internal struct Buffer<TElement>
        {
            internal TElement[] items;
            internal int count;

            internal Buffer(IEnumerable<TElement> source)
            {
                var array = (TElement[])null;
                int length = 0;
                var collection = source as ICollection<TElement>;
                if (collection != null)
                {
                    length = collection.Count;
                    if (length > 0)
                    {
                        array = new TElement[length];
                        collection.CopyTo(array, 0);
                    }
                }
                else
                {
                    foreach (TElement element in source)
                    {
                        if (array == null)
                            array = new TElement[4];
                        else if (array.Length == length)
                        {
                            var elementArray = new TElement[checked(length * 2)];
                            Array.Copy(array, 0, elementArray, 0, length);
                            array = elementArray;
                        }
                        array[length] = element;
                        ++length;
                    }
                }
                items = array;
                count = length;
            }

            internal TElement[] ToArray()
            {
                if (count == 0)
                    return new TElement[0];
                if (items.Length == count)
                    return items;
                var elementArray = new TElement[count];
                Array.Copy(items, 0, elementArray, 0, count);
                return elementArray;
            }
        }

        public static bool IsTypeInheritFrom(Type type, string parentType)
        {
            while (type != null)
            {
                if (type.FullName == parentType)
                {
                    return true;
                }
                type = type.GetTypeInfo().BaseType;
            }

            return false;
        }
    }
}
