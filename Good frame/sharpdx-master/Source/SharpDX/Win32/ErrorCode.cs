
namespace SharpDX.Win32
{
    public partial class ErrorCodeHelper
    {
        public static Result ToResult(ErrorCode errorCode)
        {
            return ToResult((int)errorCode);
        }

        public static Result ToResult(int errorCode)
        {
            return new Result(((errorCode <= 0) ? unchecked((uint)errorCode) : ((unchecked((uint)errorCode) & 0x0000FFFF) | 0x80070000)));
        }
    }
}

