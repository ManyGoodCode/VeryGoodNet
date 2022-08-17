using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CacheCow.Client.FileCacheStore;
using CacheCow.Common;

namespace CacheCow.Client.Application
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            FileStore fileStore = new FileStore("cache");
            fileStore.AddOrUpdateAsync(new CacheKey("www.baidu.com", new string[2] { "111","333"}), new System.Net.Http.HttpResponseMessage());

            //fileStore.TryRemoveAsync(new CacheKey("www.baidu.com", new string[2] { "111", "333" }));

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}
