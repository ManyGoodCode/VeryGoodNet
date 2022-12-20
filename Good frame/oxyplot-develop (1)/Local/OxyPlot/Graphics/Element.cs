
namespace OxyPlot
{
    public abstract partial class Element
    {
        public Model Parent { get; internal set; }
        public HitTestResult HitTest(HitTestArguments args)
        {
            return this.HitTestOverride(args);
        }

        protected virtual HitTestResult HitTestOverride(HitTestArguments args)
        {
            return null;
        }
    }
}
