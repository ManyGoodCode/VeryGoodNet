namespace SharpDX.Animation
{
    public partial class Timer
    {
        public bool IsEnabled
        {
            get { return IsEnabled_().Success; }
        }
    }
}