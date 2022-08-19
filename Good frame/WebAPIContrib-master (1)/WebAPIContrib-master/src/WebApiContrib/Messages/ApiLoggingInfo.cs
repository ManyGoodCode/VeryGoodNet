using System.Collections.Generic;
using System.Net;

namespace WebApiContrib.Messages
{
    public class ApiLoggingInfo
    {
        private List<string> headers = new List<string>();
        public string HttpMethod { get; set; }
        public string UriAccessed { get; set; }
        public string BodyContent { get; set; }
        public System.Net.HttpStatusCode ResponseStatusCode { get; set; }
        public string ResponseStatusMessage { get; set; }
        public string IpAddress { get; set; }
        public WebApiContrib.Messages.HttpMessageType MessageType { get; set; }

        public List<string> Headers
        {
            get { return headers; }
        }
    }

    public enum HttpMessageType
    {
        Request,
        Response
    }
}
