using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpDX
{
    public sealed class ResultDescriptor
    {
        private static readonly object LockDescriptor = new object();
        private static readonly List<Type> RegisteredDescriptorProvider = new List<Type>();
        private static readonly Dictionary<Result, ResultDescriptor> Descriptors = new Dictionary<Result, ResultDescriptor>();
        private const string UnknownText = "Unknown";

        public ResultDescriptor(
            Result code,
            string module,
            string nativeApiCode,
            string apiCode,
            string description = null)
        {
            Result = code;
            Module = module;
            NativeApiCode = nativeApiCode;
            ApiCode = apiCode;
            Description = description;
        }

        public Result Result { get; private set; }

        public int Code
        {
            get { return Result.Code; }
        }

        // Gets the module (ex: SharpDX.Direct2D1)
        public string Module { get; private set; }

        // Gets the native API code (ex: D2D1_ERR_ ...)
        public string NativeApiCode { get; private set; }

        // Gets the API code (ex: DeviceRemoved ...)
        public string ApiCode { get; private set; }

        public string Description { get; set; }

        public bool Equals(ResultDescriptor other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return other.Result.Equals(this.Result);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(ResultDescriptor))
                return false;
            return Equals((ResultDescriptor)obj);
        }

        public override int GetHashCode()
        {
            return this.Result.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("HRESULT: [0x{0:X}], Module: [{1}], ApiCode: [{2}/{3}], Message: {4}",
                this.Result.Code, this.Module, this.NativeApiCode, this.ApiCode, this.Description);
        }

        public static implicit operator Result(ResultDescriptor result)
        {
            return result.Result;
        }

        public static explicit operator int(ResultDescriptor result)
        {
            return result.Result.Code;
        }

        public static explicit operator uint(ResultDescriptor result)
        {
            return unchecked((uint)result.Result.Code);
        }

        public static bool operator ==(ResultDescriptor left, Result right)
        {
            if (left == null)
                return false;
            return left.Result.Code == right.Code;
        }

        public static bool operator !=(ResultDescriptor left, Result right)
        {
            if (left == null)
                return false;
            return left.Result.Code != right.Code;
        }

        public static void RegisterProvider(Type descriptorsProviderType)
        {
            lock (LockDescriptor)
            {
                if (!RegisteredDescriptorProvider.Contains(descriptorsProviderType))
                    RegisteredDescriptorProvider.Add(descriptorsProviderType);
            }
        }

        public static ResultDescriptor Find(Result result)
        {
            ResultDescriptor descriptor;
            lock (LockDescriptor)
            {
                if (RegisteredDescriptorProvider.Count > 0)
                {
                    foreach (var type in RegisteredDescriptorProvider)
                    {
                        AddDescriptorsFromType(type);
                    }

                    RegisteredDescriptorProvider.Clear();
                }
                if (!Descriptors.TryGetValue(result, out descriptor))
                {
                    descriptor = new ResultDescriptor(result, UnknownText, UnknownText, UnknownText);
                }
                if (descriptor.Description == null)
                {
                    var description = GetDescriptionFromResultCode(result.Code);
                    descriptor.Description = description ?? UnknownText;
                }
            }

            return descriptor;
        }

        private static void AddDescriptorsFromType(Type type)
        {
            foreach (var field in type.GetTypeInfo().DeclaredFields)
            {
                if (field.FieldType == typeof(ResultDescriptor) && field.IsPublic && field.IsStatic)
                {
                    var descriptor = (ResultDescriptor)field.GetValue(null);
                    if (!Descriptors.ContainsKey(descriptor.Result))
                    {
                        Descriptors.Add(descriptor.Result, descriptor);
                    }
                }
            }
        }

        private static string GetDescriptionFromResultCode(int resultCode)
        {
            const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
            const int FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
            const int FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

            IntPtr buffer = IntPtr.Zero;
            FormatMessageW(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, IntPtr.Zero, resultCode, 0, ref buffer, 0, IntPtr.Zero);
            var description = Marshal.PtrToStringUni(buffer);
            Marshal.FreeHGlobal(buffer);
            return description;
        }

        [DllImport("kernel32.dll", EntryPoint = "FormatMessageW")]
        private static extern uint FormatMessageW(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, ref IntPtr lpBuffer, int nSize, IntPtr Arguments);
    }
}