namespace OxyPlot
{
    public static class InterpolationAlgorithms
    {
        public static IInterpolationAlgorithm CanonicalSpline { get; } = new CanonicalSpline(0.5);

        public static IInterpolationAlgorithm CatmullRomSpline { get; } = new CatmullRomSpline(0.5);

        public static IInterpolationAlgorithm UniformCatmullRomSpline { get; } = new CatmullRomSpline(0.0);

        public static IInterpolationAlgorithm ChordalCatmullRomSpline { get; } = new CatmullRomSpline(1.0);
    }
}