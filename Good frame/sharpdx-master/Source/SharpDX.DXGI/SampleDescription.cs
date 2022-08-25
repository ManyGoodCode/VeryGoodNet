namespace SharpDX.DXGI
{
    public partial struct SampleDescription
    {
        public SampleDescription(int count, int quality)
        {
            this.Count = count;
            this.Quality = quality;
        }

        public override string ToString()
        {
            return string.Format("{{{0}, {1}}}", this.Count, this.Quality);
        }
    }
}