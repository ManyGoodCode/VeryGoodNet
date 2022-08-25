namespace SharpDX.Direct3D
{
    public partial class Blob
    {
        public static implicit operator DataPointer(Blob blob)
        {
            return new DataPointer(blob.BufferPointer, blob.BufferSize);
        }
    }
}