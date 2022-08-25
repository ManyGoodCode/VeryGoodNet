using System;
using System.Runtime.InteropServices;

namespace SharpDX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Result : IEquatable<Result>
    {
        private int _code;

        public Result(int code)
        {
            _code = code;
        }

        public Result(uint code)
        {
            _code = unchecked((int)code);
        }

        public int Code
        {
            get { return _code; }
        }

        public bool Success
        {
            get { return Code >= 0; }
        }

        public bool Failure
        {
            get { return Code < 0; }
        }

        public static explicit operator int(Result result)
        {
            return result.Code;
        }

        public static explicit operator uint(Result result)
        {
            return unchecked((uint)result.Code);
        }

        public static implicit operator Result(int result)
        {
            return new Result(result);
        }

        public static implicit operator Result(uint result)
        {
            return new Result(result);
        }


        public bool Equals(Result other)
        {
            return this.Code == other.Code;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Result))
                return false;
            return Equals((Result) obj);
        }

        public override int GetHashCode()
        {
            return Code;
        }

        public static bool operator ==(Result left, Result right)
        {
            return left.Code == right.Code;
        }

        public static bool operator !=(Result left, Result right)
        {
            return left.Code != right.Code;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "HRESULT = 0x{0:X}", _code);
        }

        public void CheckError()
        {
            if (_code < 0)
            {
                throw new SharpDXException(this);
            }
        }

        public static Result GetResultFromException(Exception ex)
        {
            return new Result(Marshal.GetHRForException(ex));
        }

        public static Result GetResultFromWin32Error(int win32Error)
        {
            const int FACILITY_WIN32 = 7;
            return win32Error <= 0 ? win32Error : (int)((win32Error & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000);
        } 

        public readonly static Result Ok = new Result(unchecked((int)0x00000000));

        public readonly static Result False = new Result(unchecked((int)0x00000001));

        public static readonly SharpDX.ResultDescriptor Abort 
            = new SharpDX.ResultDescriptor(unchecked((int)0x80004004), "General", "E_ABORT", "Operation aborted");

        public static readonly SharpDX.ResultDescriptor AccessDenied 
            = new SharpDX.ResultDescriptor(unchecked((int)0x80070005), "General", "E_ACCESSDENIED", "General access denied error");

        public static readonly SharpDX.ResultDescriptor Fail 
            = new SharpDX.ResultDescriptor(unchecked((int)0x80004005), "General", "E_FAIL", "Unspecified error");

        public static readonly SharpDX.ResultDescriptor Handle
            = new SharpDX.ResultDescriptor(unchecked((int)0x80070006), "General", "E_HANDLE", "Invalid handle");

        public static readonly SharpDX.ResultDescriptor InvalidArg 
            = new SharpDX.ResultDescriptor(unchecked((int)0x80070057), "General", "E_INVALIDARG", "Invalid Arguments");

        public static readonly SharpDX.ResultDescriptor NoInterface
            = new SharpDX.ResultDescriptor(unchecked((int)0x80004002), "General", "E_NOINTERFACE", "No such interface supported");

        public static readonly SharpDX.ResultDescriptor NotImplemented 
            = new SharpDX.ResultDescriptor(unchecked((int)0x80004001), "General", "E_NOTIMPL", "Not implemented");

        public static readonly SharpDX.ResultDescriptor OutOfMemory 
            = new SharpDX.ResultDescriptor(unchecked((int)0x8007000E), "General", "E_OUTOFMEMORY", "Out of memory");

        public static readonly SharpDX.ResultDescriptor InvalidPointer 
            = new SharpDX.ResultDescriptor(unchecked((int)0x80004003), "General", "E_POINTER", "Invalid pointer");

        public static readonly SharpDX.ResultDescriptor UnexpectedFailure
            = new SharpDX.ResultDescriptor(unchecked((int)0x8000FFFF), "General", "E_UNEXPECTED", "Catastrophic failure");

        public static readonly SharpDX.ResultDescriptor WaitAbandoned 
            = new SharpDX.ResultDescriptor(unchecked((int)0x00000080L), "General", "WAIT_ABANDONED", "WaitAbandoned");

        public static readonly SharpDX.ResultDescriptor WaitTimeout 
            = new SharpDX.ResultDescriptor(unchecked((int)0x00000102L), "General", "WAIT_TIMEOUT", "WaitTimeout");

        public static readonly SharpDX.ResultDescriptor Pending 
            = new SharpDX.ResultDescriptor(unchecked((int)0x8000000AL), "General", "E_PENDING", "Pending");
    }
}