namespace SharpDX.Animation
{
    public partial class Transition
    {
        public Transition(TransitionFactory factory, SharpDX.Animation.Interpolator interpolator)
        {
            factory.CreateTransition(interpolator, this);
        }

        public bool IsDurationKnown
        {
            get { return IsDurationKnown_().Success; }
        }
    }
}