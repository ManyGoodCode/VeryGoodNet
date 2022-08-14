namespace OxyPlot.Utilities
{
    public static class Helpers
    {
        public static void Swap<T>(ref T value, ref T other)
        {
            var tmp = value;
            value = other;
            other = tmp;
        }
    }
}
