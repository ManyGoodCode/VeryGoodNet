using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CacheCow.Common;

namespace CacheCow.Client
{
    /// <summary>
    /// 实现 HttpResponseMessage ， HttpRequestMessage 与 Stream 的序列化和反序列化
    /// </summary>
    public class MessageContentHttpMessageSerializer : IHttpMessageSerializerAsync
    {
        private bool bufferContent;

        public MessageContentHttpMessageSerializer()
            : this(bufferContent: true) { }

        public MessageContentHttpMessageSerializer(bool bufferContent)
        {
            this.bufferContent = bufferContent;
        }

        public async Task SerializeAsync(HttpResponseMessage response, Stream stream)
        {
            if (response.Content != null)
            {
                TraceWriter.WriteLine("SerializeAsync - before load", TraceLevel.Verbose);
                if (bufferContent)
                    await response.Content.LoadIntoBufferAsync().ConfigureAwait(false);
                TraceWriter.WriteLine("SerializeAsync - after load", TraceLevel.Verbose);
            }
            else
            {
                TraceWriter.WriteLine("Content NULL - before load", TraceLevel.Verbose);
            }

            HttpMessageContent httpMessageContent = new HttpMessageContent(httpResponse: response);
            byte[] buffer = await httpMessageContent.ReadAsByteArrayAsync();
            TraceWriter.WriteLine("SerializeAsync - after ReadAsByteArrayAsync", TraceLevel.Verbose);
            stream.Write(buffer, 0, buffer.Length);
        }

        public async Task SerializeAsync(HttpRequestMessage request, Stream stream)
        {
            if (request.Content != null && bufferContent)
            {
                await request.Content.LoadIntoBufferAsync().ConfigureAwait(false);
            }

            HttpMessageContent httpMessageContent = new HttpMessageContent(request);
            byte[] buffer = await httpMessageContent.ReadAsByteArrayAsync().ConfigureAwait(false);
            stream.Write(buffer, 0, buffer.Length);
        }

        public async Task<HttpResponseMessage> DeserializeToResponseAsync(Stream stream)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StreamContent(stream);
            response.Content.Headers.Add(HttpHeaderNames.ContentType, "application/http;msgtype=response");
            TraceWriter.WriteLine("before ReadAsHttpResponseMessageAsync", TraceLevel.Verbose);
            HttpResponseMessage responseMessage = await response.Content.ReadAsHttpResponseMessageAsync().ConfigureAwait(false);
            if (responseMessage.Content != null && bufferContent)
            {
                await responseMessage.Content.LoadIntoBufferAsync().ConfigureAwait(false);
            }

            if (responseMessage.Content == null)
                TraceWriter.WriteLine("Content is NULL desering from cache", TraceLevel.Warning);

            return responseMessage;
        }

        public async Task<HttpRequestMessage> DeserializeToRequestAsync(Stream stream)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Content = new StreamContent(stream);
            request.Content.Headers.Add(HttpHeaderNames.ContentType, "application/http;msgtype=request");
            HttpRequestMessage requestMessage = await request.Content.ReadAsHttpRequestMessageAsync().ConfigureAwait(false);
            if (requestMessage.Content != null && bufferContent)
                await requestMessage.Content.LoadIntoBufferAsync().ConfigureAwait(false);
            return requestMessage;
        }
    }
}
