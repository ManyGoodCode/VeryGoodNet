using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace CacheCow
{
    /// <summary>
    /// 环境变量 设置Trace等级
    /// 1. 编辑 Environment Variable 环境变量 【环境变量要重启电脑后才生效】
    /// </summary>
	internal static class TraceWriter
    {
        static TraceWriter()
        {
            ExamineEnvVar();
        }

		public const string CacheCowTraceSwitch = "CacheCow";
        public const string CacheCowTracingEnvVarName = "CacheCow.Tracing.Switch";

		internal static readonly TraceSwitch switchTrace = new TraceSwitch(CacheCowTraceSwitch, "CacheCow Trace Switch");

        private static void ExamineEnvVar()
        {
            // 1. 编辑 Environment Variable 环境变量 【环境变量要重启电脑后才生效】
            string envvarValue = Environment.GetEnvironmentVariable(CacheCowTracingEnvVarName) ?? "";
            if (envvarValue.Length > 0)
            {
                TraceLevel level;
                if (Enum.TryParse(envvarValue, out level))
                    switchTrace.Level = level;
            }
        }

        public static void WriteLine(string message, TraceLevel level, params object[] args)
        {

            if (switchTrace.Level < level)
				return;

			string dateTimeOfEvent = DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffff");
			string callingMethod = string.Empty;
            try
            {
                callingMethod = new StackFrame(1).GetMethod().Name;
            }
            catch
            { }
			Trace.WriteLine(string.Format("{0} - {1}: {2}",
				dateTimeOfEvent,
				callingMethod,
				args.Length == 0 ? message : string.Format(message, args)
				));

		}
	}
}
