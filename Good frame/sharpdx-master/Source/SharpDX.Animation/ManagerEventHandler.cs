namespace SharpDX.Animation
{
    [Shadow(typeof(ManagerEventHandlerShadow))]
    internal partial interface ManagerEventHandler
    {
        void OnManagerStatusChanged(SharpDX.Animation.ManagerStatus newStatus, SharpDX.Animation.ManagerStatus previousStatus);
    }
}