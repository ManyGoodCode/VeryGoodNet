using System;

namespace SharpDX.Animation
{
    public partial class TransitionFactory
    {
        private static readonly Guid TransitionFactoryGuid = new Guid("8A9B1CDD-FCD7-419c-8B44-42FD17DB1887");

        public TransitionFactory()
        {
            Utilities.CreateComInstance(TransitionFactoryGuid, Utilities.CLSCTX.ClsctxInprocServer, Utilities.GetGuidFromType(typeof(TransitionFactory)), this);
        }
    }
}