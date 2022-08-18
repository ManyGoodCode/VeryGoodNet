using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheCow.Client
{
    /// <summary>
    /// 缓存元数据项。包含:字节数组Key,最近访问时间LastAccessed,数据大小Size,作用域Domain
    /// </summary>
	public class CacheItemMetadata
	{
		public Byte[] Key { get; set; }
		public DateTime LastAccessed { get; set; }
		public long Size { get; set; }
		public string Domain { get; set; }
	}
}
