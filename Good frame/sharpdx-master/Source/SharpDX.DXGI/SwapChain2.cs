namespace SharpDX.DXGI
{
    public partial class SwapChain2
    {	
        public Size2 SourceSize
        {
            get
            {
                int width;
                int height;
                GetSourceSize(out width, out height);
                return new Size2(width, height);
            }
            set
            {
                SetSourceSize(value.Width, value.Height);
            }
        }
    }
}