namespace SharpDX.Multimedia
{
    public static class SpeakersExtensions
    {
        public static int ToChannelCount(Speakers speakers)
        {
            int channelsMask = (int)speakers;
            int channelCount = 0;
            while (channelsMask != 0)
            {
                if ((channelsMask & 1) != 0)
                    channelCount++;
                channelsMask >>= 1;
            }

            return channelCount;
        }
    }
}

