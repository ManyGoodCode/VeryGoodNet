using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CacheCow.Common;

namespace CacheCow.Client.FileCacheStore
{
    public class FileStore : ICacheStore
    {
        private readonly MessageContentHttpMessageSerializer _serializer = new MessageContentHttpMessageSerializer();
        private readonly string _cacheRoot;

        public TimeSpan MinExpiry { get; set; }

        private static readonly List<string> ForbiddenDirectories =
            new List<string>()
            {
                "/",
                "",
                ".",
                ".."
            };

        public FileStore(string cacheRoot)
        {
            if (cacheRoot is null || ForbiddenDirectories.Contains(cacheRoot))
            {
                throw new ArgumentException(
                    "The given caching directory is null or invalid. Do give an explicit caching directory, not empty, '/' or '.'. This will prevent accidents when cleaning the cache");
            }

            _cacheRoot = cacheRoot;
            if (!Directory.Exists(_cacheRoot))
            {
                Directory.CreateDirectory(cacheRoot);
            }
        }

        public async Task<HttpResponseMessage> GetValueAsync(CacheKey key)
        {
            if (!File.Exists(PathFor(key)))
            {
                return null;
            }

            using (FileStream fs = File.OpenRead(PathFor(key)))
            {
                return await _serializer.DeserializeToResponseAsync(fs);
            }
        }

        /// <summary>
        /// 通过CacheKey加密生成文件名 记录请求信息到本地文件
        /// 例如:cache/C4TsCs8GI8zAVxFA78HKWZFZS
        /// </summary>
        public async Task AddOrUpdateAsync(CacheKey key, HttpResponseMessage response)
        {
            using (FileStream fs = File.OpenWrite(PathFor(key)))
            {
                await _serializer.SerializeAsync(response, fs);
            }
        }

        /// <summary>
        /// 删除CacheKey加密生成的本地日志
        /// </summary>
        public async Task<bool> TryRemoveAsync(CacheKey key)
        {
            if (!File.Exists(PathFor(key)))
            {
                return false;
            }

            File.Delete(PathFor(key));
            return true;
        }

        /// <summary>
        /// 删除Cache下的所有日志
        /// </summary>
        public async Task ClearAsync()
        {
            foreach (string f in Directory.GetFiles(_cacheRoot))
            {
                File.Delete(f);
            }
        }

        private string PathFor(CacheKey key)
        {
            return _cacheRoot + "/" + key.HashBase64.Replace('/', '!');
        }

        public void Dispose()
        {
        }

        public bool IsEmpty()
        {
            return Directory.GetFiles(_cacheRoot).Length == 0;
        }
    }
}
