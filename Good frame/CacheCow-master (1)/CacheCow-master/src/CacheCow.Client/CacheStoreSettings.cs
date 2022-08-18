using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheCow.Client
{
    /// <summary>
    /// 缓存器设置。最大可以存储的字节长度TotalQuota为0则无限制；单位 Domain 容量 50M
    /// </summary>
    public class CacheStoreSettings
    {
        public CacheStoreSettings()
        {
            TotalQuota = long.MaxValue;
            PerDomainQuota = 50 * 1024 * 1024; // 50 MB
        }

        public long TotalQuota { get; set; }
        public long PerDomainQuota { get; set; }
    }
}
