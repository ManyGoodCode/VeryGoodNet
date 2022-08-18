using System;
using System.Collections.Generic;
using System.Text;

namespace CacheCow.Server
{
    public interface IHasher : IDisposable
    {
        /// <summary>
        /// 生成 hash
        /// </summary>
        string ComputeHash(byte[] bytes);
    }
}
