using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiContrib.Serialization
{
    public class MessageContentHttpMessageSerializer : IHttpMessageSerializerAsync
    {
        private bool bufferContent;

        public MessageContentHttpMessageSerializer()
            : this(false)
        {
        }

        public MessageContentHttpMessageSerializer(bool bufferContent)
        {
            this.bufferContent = bufferContent;
        }

        public async Task SerializeAsync(Task<HttpResponseMessage> response, Stream stream)
        {
            HttpResponseMessage r = await response;
            if (r.Content != null)
            {
                await r.Content.LoadIntoBufferAsync();
                HttpMessageContent httpMessageContent = new HttpMessageContent(r);
                byte[] buffer = await httpMessageContent.ReadAsByteArrayAsync();
                await Task.Factory.FromAsync(
                    stream.BeginWrite, 
                    stream.EndWrite,
                    buffer, 
                    0, 
                    buffer.Length, 
                    null, 
                    TaskCreationOptions.AttachedToParent);
            }
            else
            {
                HttpMessageContent httpMessageContent = new HttpMessageContent(r);
                byte[] buffer = await httpMessageContent.ReadAsByteArrayAsync();
                await Task.Factory.FromAsync(
                    stream.BeginWrite, 
                    stream.EndWrite,
                    buffer, 
                    0, 
                    buffer.Length, 
                    null, 
                    TaskCreationOptions.AttachedToParent);
            }
        }

        public async Task SerializeAsync(HttpRequestMessage request, Stream stream)
        {
            if (request.Content != null)
            {
                await request.Content.LoadIntoBufferAsync();
                HttpMessageContent httpMessageContent = new HttpMessageContent(request);
                byte[] buffer = await httpMessageContent.ReadAsByteArrayAsync();
                await Task.Factory.FromAsync(
                    stream.BeginWrite, 
                    stream.EndWrite,
                    buffer, 
                    0, 
                    buffer.Length, 
                    null, 
                    TaskCreationOptions.AttachedToParent);
            }
            else
            {
                HttpMessageContent httpMessageContent = new HttpMessageContent(request);
                byte[] buffer = await httpMessageContent.ReadAsByteArrayAsync();
                await Task.Factory.FromAsync(
                    stream.BeginWrite, 
                    stream.EndWrite,
                    buffer, 
                    0, 
                    buffer.Length, 
                    null, 
                    TaskCreationOptions.AttachedToParent);
            }
        }

        public Task<HttpResponseMessage> DeserializeToResponseAsync(Stream stream)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StreamContent(stream);
            response.Content.Headers.Add("Content-Type", "application/http;msgtype=response");
            return response.Content.ReadAsHttpResponseMessageAsync();
        }

        public Task<HttpRequestMessage> DeserializeToRequestAsync(Stream stream)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Content = new StreamContent(stream);
            request.Content.Headers.Add("Content-Type", "application/http;msgtype=request");
            return request.Content.ReadAsHttpRequestMessageAsync();
        }
    }
}
