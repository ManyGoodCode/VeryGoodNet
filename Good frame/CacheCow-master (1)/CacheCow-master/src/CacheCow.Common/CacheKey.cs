using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CacheCow.Common
{
    /// <summary>
    /// 通过 resourceUri 和 headerValues 生成字符串【相等性判断】。再将字符串通过SHA加密生成新的字符串
    /// </summary>
	public class CacheKey
	{
		private readonly string resourceUri;
		private readonly string toString;
		private readonly byte[] hash;
		private readonly string hashBase64;

		private const string CacheKeyFormat = "{0}-{1}";

		public CacheKey(string resourceUri, IEnumerable<string> headerValues = null)
		{
			toString = string.Format(CacheKeyFormat, resourceUri, string.Join("-", headerValues));
			using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
			{
				hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(toString));
			}

			hashBase64 = Convert.ToBase64String(hash);
            this.resourceUri = resourceUri;
		}

        public string ResourceUri
		{
			get { return resourceUri; }
		}

		public byte[] Hash
		{
			get { return hash; }
		}

		public string HashBase64
		{
			get { return hashBase64; }
		}

		public override string ToString()
		{
			return toString;
		}


		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
            CacheKey eTagKey = obj as CacheKey;
			if (eTagKey == null)
				return false;
			return ToString() == eTagKey.ToString();
		}

		public override int GetHashCode()
		{
			return toString.GetHashCode();
		}
	}
}
