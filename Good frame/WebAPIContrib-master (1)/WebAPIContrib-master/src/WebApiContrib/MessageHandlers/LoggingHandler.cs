﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebApiContrib.Data;
using WebApiContrib.Messages;

namespace WebApiContrib.MessageHandlers
{
    public class LoggingHandler : DelegatingHandler
    {
        private ILoggingRepository _repository;

        public LoggingHandler(ILoggingRepository repository)
        {
            _repository = repository;
        }

        public LoggingHandler(
            HttpMessageHandler innerHandler, 
            ILoggingRepository repository)
            : base(innerHandler)
        {
            _repository = repository;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Log the request information
            LogRequestLoggingInfo(request);

            // Execute the request
            return base.SendAsync(request, cancellationToken).ContinueWith(task =>
            {
                HttpResponseMessage response = task.Result;
                // Extract the response logging info then persist the information
                LogResponseLoggingInfo(response);
            	return response;
            });
        }

        private void LogRequestLoggingInfo(HttpRequestMessage request)
        {
            ApiLoggingInfo info = new ApiLoggingInfo();
            info.HttpMethod = request.Method.Method;
            info.UriAccessed = request.RequestUri.AbsoluteUri;
            info.IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0";
            info.MessageType = HttpMessageType.Request;
            
            ExtractMessageHeadersIntoLoggingInfo(info, request.Headers.ToList());
            if (request.Content != null)
            {
                request.Content.ReadAsByteArrayAsync()
                    .ContinueWith(task =>
                    {
                        info.BodyContent = Encoding.UTF8.GetString(task.Result);
                        _repository.Log(info);

                    });

                return;
            }

            _repository.Log(info);
        }

        private void LogResponseLoggingInfo(HttpResponseMessage response)
        {
            ApiLoggingInfo info = new ApiLoggingInfo();
            info.MessageType = HttpMessageType.Response;
            info.HttpMethod = response.RequestMessage.Method.ToString();
            info.ResponseStatusCode = response.StatusCode;
            info.ResponseStatusMessage = response.ReasonPhrase;
            info.UriAccessed = response.RequestMessage.RequestUri.AbsoluteUri;
            info.IpAddress = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "0.0.0.0";

            ExtractMessageHeadersIntoLoggingInfo(info, response.Headers.ToList());
            
            if (response.Content != null)
            {
                response.Content.ReadAsByteArrayAsync()
                    .ContinueWith(task =>
                    {
                        var responseMsg = Encoding.UTF8.GetString(task.Result);
                        info.BodyContent = responseMsg;
                        _repository.Log(info);
                    });

                return;
            }

            _repository.Log(info);
        }

        private void ExtractMessageHeadersIntoLoggingInfo(
            ApiLoggingInfo info, 
            List<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            headers.ForEach(h =>
            {
                StringBuilder headerValues = new StringBuilder();
                if (h.Value != null)
                {
                    foreach (string hv in h.Value)
                    {
                        if (headerValues.Length > 0)
                        {
                            headerValues.Append(", ");
                        }

                        headerValues.Append(hv);
                    }
                }

                info.Headers.Add(string.Format("{0}: {1}", h.Key, headerValues.ToString()));
            });
        }
    }
}