using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CacheCow.Common.Helpers
{
	public static class BasicExtensions
	{
        /// <summary>
        /// 判断一个元素是否在集合中
        /// </summary>
		public static bool IsIn<T>(this T item, params T[] list)
		{
			if (list == null || list.Length == 0)
				return false;
			return list.Any(x => EqualityComparer<T>.Default.Equals(x, item));
		}

        /// <summary>
        /// 将委托集合变成一个委托
        /// </summary>
		public static Action Chain(this IEnumerable<Action> actions)
		{
			return () =>
			{
				foreach (Action action in actions)
					action();
			};
		}

        /// <summary>
        /// 将异步委托集合变成一个异步委托
        /// </summary>
        public static Func<Task> Chain(this IEnumerable<Func<Task>> actions)
        {
            return async () =>
            {
                foreach (Func<Task> action in actions)
                    await action().ConfigureAwait(false);
            };
        }

        /// <summary>
        /// 输出十六进制字符串 0x11 表示11
        /// </summary>
		public static string ToHex(this byte[] data)
		{
            StringBuilder hex = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        /// <summary>
        /// 将字符串转换为十六进制数据 11 表示 0x11
        /// </summary>
		public static byte[] FromHex(this string hex)
		{
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
	}
}
