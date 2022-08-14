namespace OxyPlot
{
    public class OxyMouseDownEventArgs : OxyMouseEventArgs
    {
        public OxyMouseButton ChangedButton { get; set; }
        public int ClickCount { get; set; }
        public HitTestResult HitTestResult { get; set; } 
    }
}