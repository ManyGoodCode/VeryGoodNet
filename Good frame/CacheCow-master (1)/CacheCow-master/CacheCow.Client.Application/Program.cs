using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CacheCow.Client.FileCacheStore;
using CacheCow.Client.Headers;
using CacheCow.Common;
using CacheCow.Common.Helpers;

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
            //FileStore fileStore = new FileStore("cache");
            //fileStore.AddOrUpdateAsync(new CacheKey("www.baidu.com", new string[2] { "111","333"}), new System.Net.Http.HttpResponseMessage());
            //fileStore.TryRemoveAsync(new CacheKey("www.baidu.com", new string[2] { "111", "333" }));


            //byte[] datas = new byte[3] { 0x11, 0x22, 0x34 };
            //string hexString = datas.ToHex();
            //byte[] datas1 = hexString.FromHex();

            //CacheCowHeader head = new CacheCowHeader();
            //head.WasStale = true;
            //head.DidNotExist = true;
            //head.NotCacheable = false;
            //head.CacheValidationApplied = false;
            //head.RetrievedFromCache = true;
            //string str = head.ToString();

            //CacheCowHeader head2;
            //CacheCowHeader.TryParse(str,out head2);

            //DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.Now);
            //Thread.Sleep(300);
            //int ms = dateTimeOffset.Millisecond;
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            TraceSwitch switchTrace = new TraceSwitch("CacheCow", "CacheCow Trace Switch");
            string CacheCowTracingEnvVarName = "CacheCow.Tracing.Switch";

            string envvarValue = Environment.GetEnvironmentVariable(CacheCowTracingEnvVarName) ?? "";
            if (envvarValue.Length > 0)
            {
                TraceLevel level;
                if (Enum.TryParse(envvarValue, out level))
                    switchTrace.Level = level;
            }
        }

        static Task<int> Add()
        {
            TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();
            taskCompletionSource.SetResult(1);
            return taskCompletionSource.Task;
        }
    }
}
