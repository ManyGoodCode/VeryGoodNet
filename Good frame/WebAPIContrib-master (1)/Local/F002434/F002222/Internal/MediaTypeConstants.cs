using System.Net.Http.Headers;

namespace F002222.Internal
{
    public static class MediaTypeConstants
    {
        private static readonly MediaTypeHeaderValue _applicationJson = new MediaTypeHeaderValue("application/json");

        public static MediaTypeHeaderValue ApplicationJson { get { return _applicationJson; } }
    }
}