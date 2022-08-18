using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CacheCow.Client.Headers
{
    public class CacheCowHeader
    {
        /// <summary>
        /// 常量:x-cachecow-client
        /// </summary>
        public const string Name = "x-cachecow-client";

        public static class ExtensionNames
        {
            public const string WasStale = "was-stale";
            public const string DidNotExist = "did-not-exist";
            public const string NotCacheable = "not-cacheable";
            public const string CacheValidationApplied = "cache-validation-applied";
            public const string RetrievedFromCache = "retrieved-from-cache";
        }

        public string Version { get; private set; }
        public bool? WasStale { get; set; }
        public bool? DidNotExist { get; set; }
        public bool? NotCacheable { get; set; }
        public bool? CacheValidationApplied { get; set; }
        public bool? RetrievedFromCache { get; set; }

        /// <summary>
        /// 读取当前运行程序集dll的版本
        /// </summary>
		public CacheCowHeader()
        {
            Version = Assembly.GetExecutingAssembly()
                .GetName().Version.ToString();
        }

        /// <summary>
        /// 组合成的字符串格式
        /// 2.11.1.0;was-stale=true;not-cacheable=false;did-not-exist=true;cache-validation-applied=false;retrieved-from-cache=true
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Version);
            AddToStringBuilder(sb, WasStale, ExtensionNames.WasStale);
            AddToStringBuilder(sb, NotCacheable, ExtensionNames.NotCacheable);
            AddToStringBuilder(sb, DidNotExist, ExtensionNames.DidNotExist);
            AddToStringBuilder(sb, CacheValidationApplied, ExtensionNames.CacheValidationApplied);
            AddToStringBuilder(sb, RetrievedFromCache, ExtensionNames.RetrievedFromCache);
            return sb.ToString();
        }

        private void AddToStringBuilder(StringBuilder sb, bool? property, string extensionName)
        {
            if (property == null)
                return;
            sb.Append(';');
            sb.Append(extensionName);
            sb.Append('=');
            sb.Append(property.Value.ToString().ToLower());
        }

        /// <summary>
        /// 根据字符串解析 生成一个CacheCowHeader对象
        /// </summary>
        public static bool TryParse(string value, out CacheCowHeader cacheCowHeader)
        {
            cacheCowHeader = null;
            if (value == null)
                return false;
            if (value == string.Empty)
                return false;
            cacheCowHeader = new CacheCowHeader();
            string[] chunks = value.Split(new[] { ";" }, StringSplitOptions.None);
            cacheCowHeader.Version = chunks[0];
            for (int i = 1; i < chunks.Length; i++)
            {
                // 默认都会执行，因为构造的对象默认为null。顺序不能打乱 否则赋值错误
                cacheCowHeader.WasStale = cacheCowHeader.WasStale ?? ParseNameValue(chunks[i], ExtensionNames.WasStale);
                cacheCowHeader.CacheValidationApplied = cacheCowHeader.CacheValidationApplied ?? ParseNameValue(chunks[i], ExtensionNames.CacheValidationApplied);
                cacheCowHeader.NotCacheable = cacheCowHeader.NotCacheable ?? ParseNameValue(chunks[i], ExtensionNames.NotCacheable);
                cacheCowHeader.DidNotExist = cacheCowHeader.DidNotExist ?? ParseNameValue(chunks[i], ExtensionNames.DidNotExist);
                cacheCowHeader.RetrievedFromCache = cacheCowHeader.RetrievedFromCache ?? ParseNameValue(chunks[i], ExtensionNames.RetrievedFromCache);
            }

            return true;
        }

        /// <summary>
        /// 解析 entry 是否为对应的name,是的话 得到 name标识的值
        /// </summary>
        private static bool? ParseNameValue(string entry, string name)
        {
            if (string.IsNullOrEmpty(entry))
                return null;

            string[] chunks = entry.Split('=');
            if (chunks.Length != 2)
                return null;

            chunks[0] = chunks[0].Trim();
            chunks[1] = chunks[1].Trim();

            if (chunks[0].ToLower() != name)
                return null;

            bool result = false;
            if (!bool.TryParse(chunks[1], out result))
                return null;

            return result;
        }
    }
}
