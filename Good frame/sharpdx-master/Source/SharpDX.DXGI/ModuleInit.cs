namespace SharpDX.DXGI
{
    class ModuleInit
    {
        /// </remarks>
        [Tag("SharpDX.ModuleInit")]
        internal static void Setup()
        {
            ResultDescriptor.RegisterProvider(typeof(ResultCode));
        }
    }
}