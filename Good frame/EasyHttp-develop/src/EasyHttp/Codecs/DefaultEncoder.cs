using System.Text;
using JsonFx.Serialization;
using JsonFx.Serialization.Providers;

namespace EasyHttp.Codecs
{
    public class DefaultEncoder : IEncoder
    {
        readonly IDataWriterProvider dataWriterProvider;

        public DefaultEncoder(IDataWriterProvider dataWriterProvider)
        {
            this.dataWriterProvider = dataWriterProvider;
        }

        public byte[] Encode(object input, string contentType)
        {
            if (input.GetType() == typeof(string))
            {
                return Encoding.UTF8.GetBytes((string)input);
            }

            IDataWriter serializer = dataWriterProvider.Find(contentType, contentType);
            if (serializer == null)
            {
                throw new SerializationException("The encoding requested does not have a corresponding encoder");
            }

            string serialized = serializer.Write(input);
            return Encoding.UTF8.GetBytes(serialized);
        }

       
    }
}