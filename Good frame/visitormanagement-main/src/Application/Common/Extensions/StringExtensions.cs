
using System;
using System.Text;

namespace CleanArchitecture.Blazor.Application.Common.Extensions
{
    /// <summary>
    /// 通过MD5对字符串进行加密形成新的字符串。[解密方法另外探究]
    /// </summary>
    public static class StringExtensions
    {
        public static string ToMD5(this string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                Encoding encoding = Encoding.ASCII;
                byte[] data = encoding.GetBytes(input);

                Span<byte> hashBytes = stackalloc byte[16];
                md5.TryComputeHash(data, hashBytes, out int written);
                if (written != hashBytes.Length)
                    throw new OverflowException();

                Span<char> stringBuffer = stackalloc char[32];
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashBytes[i].TryFormat(stringBuffer.Slice(2 * i), out _, "x2");
                }

                return new string(stringBuffer);
            }
        }
    }
}
