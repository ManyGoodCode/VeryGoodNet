﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApiContrib.Content;

namespace WebApiContrib.MessageHandlers
{
    public class EncodingHandler : DelegatingHandler
    {
        public EncodingHandler()
        {
        }

        public EncodingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string contentEncodingHeader = "Content-Encoding";
            string encoding = "gzip";
            if (!request.Content.Headers.Contains(contentEncodingHeader))
            {
                request.Content = new CompressedContent(request.Content, encoding);
            }
            else
            {
                HttpContent originalContent = request.Content;
                var incomingStream = request.Content.ReadAsStreamAsync().Result;
                incomingStream.Position = 0;

                MemoryStream outputStream = new MemoryStream(4096);
                using (GZipStream decompressedStream = new GZipStream(incomingStream, CompressionMode.Decompress, leaveOpen: false))
                {
                    decompressedStream.CopyTo(outputStream);
                }

                outputStream.Position = 0;
                StreamContent newContent = new StreamContent(outputStream);
                foreach (KeyValuePair<string, IEnumerable<string>> header in originalContent.Headers)
                {
                    newContent.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                request.Content = newContent;
            }

            return base.SendAsync(request, cancellationToken).ContinueWith<HttpResponseMessage>((responseToCompleteTask) =>
            {
                HttpResponseMessage response = responseToCompleteTask.Result;
                if (response.RequestMessage.Headers.AcceptEncoding != null && response.RequestMessage.Headers.AcceptEncoding.Count > 0)
                {
                    var encodingType = response.RequestMessage.Headers.AcceptEncoding.First().Value;
                    response.Content = new CompressedContent(response.Content, encodingType);
                }

                return response;
            },
            TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
