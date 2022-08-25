using System;

namespace SharpDX.Animation
{
    public partial class TransitionLibrary
    {
        private static readonly Guid TransitionLibraryGuid = new Guid("1D6322AD-AA85-4EF5-A828-86D71067D145");
        public TransitionLibrary()
        {
            Utilities.CreateComInstance(TransitionLibraryGuid, Utilities.CLSCTX.ClsctxInprocServer, Utilities.GetGuidFromType(typeof(TransitionLibrary)), this);
        }
    }
}