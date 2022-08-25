using System;

namespace SharpDX.Animation
{
    public partial class Timer
    {
        private static readonly Guid TimerGuid = new Guid("BFCD4A0C-06B6-4384-B768-0DAA792C380E");
        public Timer()
        {
            Utilities.CreateComInstance(TimerGuid, Utilities.CLSCTX.ClsctxInprocServer, Utilities.GetGuidFromType(typeof(Timer)), this);
        }
    }
}