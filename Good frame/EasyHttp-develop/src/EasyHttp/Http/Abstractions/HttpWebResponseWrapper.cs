using System;
using System.IO;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace EasyHttp.Http.Abstractions
{
    public class HttpWebResponseWrapper : IWebResponse, IHttpWebResponse
    {
        private readonly HttpWebResponse innerResponse;

        public HttpWebResponseWrapper(HttpWebResponse innerResponse)
        {
            this.innerResponse = innerResponse;
        }

        public HttpWebResponse InnerResponse
        {
            get { return innerResponse; }
        }
        public bool IsMutuallyAuthenticated
        {
            get { return innerResponse.IsMutuallyAuthenticated; }
        }
        long IWebResponse.ContentLength { get; set; }
        string IWebResponse.ContentType { get; set; }
        public CookieCollection Cookies
        {
            get { return innerResponse.Cookies; }
            set { innerResponse.Cookies = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return innerResponse.Headers; } 
        }
        public bool SupportsHeaders 
        { 
            get { return true; } 
        }
        public long ContentLength 
        { 
            get { return innerResponse.ContentLength; } 
        }
        public string ContentEncoding 
        { 
            get { return innerResponse.ContentEncoding; } 
        }
        public string ContentType 
        {
            get { return innerResponse.ContentType; }
        }
        public string CharacterSet
        { 
            get { return innerResponse.CharacterSet; }
        }
        public string Server 
        { 
            get { return innerResponse.Server; } 
        }
        public DateTime LastModified
        { 
            get { return innerResponse.LastModified; } 
        }
        public HttpStatusCode StatusCode 
        { 
            get { return innerResponse.StatusCode; } 
        }
        public string StatusDescription 
        { 
            get { return innerResponse.StatusDescription; }
        }
        public Version ProtocolVersion 
        { 
            get { return innerResponse.ProtocolVersion; }
        }
        public Uri ResponseUri 
        {
            get { return innerResponse.ResponseUri; }
        }
        public string Method 
        { 
            get { return innerResponse.Method; } 
        }
        public bool IsFromCache
        {
            get { return innerResponse.IsFromCache; } 
        }

        public Stream GetResponseStream()
        {
            return innerResponse.GetResponseStream();
        }

        public void Close()
        {
            innerResponse.Close();
        }

        public string GetResponseHeader(string headerName)
        {
            return innerResponse.GetResponseHeader(headerName);
        }

        public object GetLifetimeService()
        {
            return innerResponse.GetLifetimeService();
        }

        public object InitializeLifetimeService()
        {
            return innerResponse.InitializeLifetimeService();
        }

        public ObjRef CreateObjRef(Type requestedType)
        {
            return innerResponse.CreateObjRef(requestedType);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            (innerResponse as ISerializable).GetObjectData(info, context);
        }

        public void Dispose()
        {
            (innerResponse as IDisposable).Dispose();
        }
    }
}