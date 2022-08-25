namespace SharpDX
{
    class ModuleInit
    {
        [Tag("SharpDX.ModuleInit")]
        internal static void Setup()
        {
            ResultDescriptor.RegisterProvider(typeof(Result));
        }
    }
}