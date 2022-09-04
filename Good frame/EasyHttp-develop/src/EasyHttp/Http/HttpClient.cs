using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using EasyHttp.Codecs;
using EasyHttp.Configuration;
using EasyHttp.Http.Abstractions;
using EasyHttp.Http.Injection;
using EasyHttp.Infrastructure;

namespace EasyHttp.Http
{
    public class HttpClient
    {
        readonly string _baseUri;
        readonly IEncoder _encoder;
        readonly IDecoder _decoder;
        readonly UriComposer _uriComposer;
        private bool _shouldRemoveAtSign = true;

        public virtual bool LoggingEnabled { get; set; }
        public virtual bool ThrowExceptionOnHttpError { get; set; }
        public virtual bool StreamResponse { get; set; }

        public virtual bool ShouldRemoveAtSign
        {
            get { return _shouldRemoveAtSign; }
            set
            {
                _shouldRemoveAtSign = value;
                _decoder.ShouldRemoveAtSign = value;
            }
        }

        public IList<HttpRequestInterception> RegisteredInterceptions { get; set; }

        public HttpClient():this(new DefaultEncoderDecoderConfiguration())
        {
        }

        public HttpClient(IEncoderDecoderConfiguration encoderDecoderConfiguration)
        {
            _encoder = encoderDecoderConfiguration.GetEncoder();
            _decoder = encoderDecoderConfiguration.GetDecoder();
            _decoder.ShouldRemoveAtSign = _shouldRemoveAtSign;
            _uriComposer = new UriComposer();

            Request = new HttpRequest(_encoder);

            RegisteredInterceptions = new List<HttpRequestInterception>();
        }

        public HttpClient(string baseUri, Func<string,HttpResponse> getResponse = null): this(new DefaultEncoderDecoderConfiguration())
        {
            _baseUri = baseUri;
        }

        public virtual HttpResponse Response { get; private set; }
        public virtual HttpRequest Request { get; private set; }

        void InitRequest(string uri, HttpMethod method, object query)
        {
            Request.Uri = _uriComposer.Compose(_baseUri, uri, query, Request.ParametersAsSegments);
            Request.Data = null;
            Request.PutFilename = String.Empty;
            Request.Expect = false;
            Request.KeepAlive = true;
            Request.MultiPartFormData = null;
            Request.MultiPartFileData = null;
            Request.ContentEncoding = null;
            Request.Method = method;
        }


        public virtual HttpResponse GetAsFile(string uri, string filename)
        {
            InitRequest(uri, HttpMethod.GET, null);
            return ProcessRequest(filename);
        }

        public virtual HttpResponse Get(string uri, object query = null)
        {
            InitRequest(uri, HttpMethod.GET, query);
            return ProcessRequest();
        }

        public virtual HttpResponse Options(string uri)
        {
            InitRequest(uri, HttpMethod.OPTIONS, null);
            return ProcessRequest();
        }

        public virtual HttpResponse Post(string uri, object data, string contentType, object query = null)
        {
            InitRequest(uri, HttpMethod.POST, query);
            InitData(data, contentType);
            return ProcessRequest();
        }

        public virtual HttpResponse Patch(string uri, object data, string contentType, object query = null)
        {
            InitRequest(uri, HttpMethod.PATCH, query);
            InitData(data, contentType);
            return ProcessRequest();
        }

        public virtual HttpResponse Post(string uri, IDictionary<string, object> formData, IList<FileData> files, object query = null)
        {
            InitRequest(uri, HttpMethod.POST, query);
            Request.MultiPartFormData = formData;
            Request.MultiPartFileData = files;
            Request.KeepAlive = true;
            return ProcessRequest();
        }

        public virtual HttpResponse Put(string uri, object data, string contentType, object query = null)
        {
            InitRequest(uri, HttpMethod.PUT, query);
            InitData(data, contentType);
            return ProcessRequest();
        }

        void InitData(object data, string contentType)
        {
            if (data != null)
            {
                Request.ContentType = contentType;
                Request.Data = data;
            }
        }

        public virtual HttpResponse Delete(string uri, object query = null)
        {
            InitRequest(uri, HttpMethod.DELETE, query);
            return ProcessRequest();
        }


        public virtual HttpResponse Head(string uri, object query = null)
        {
            InitRequest(uri, HttpMethod.HEAD, query);
            return ProcessRequest();
        }

        public virtual HttpResponse PutFile(string uri, string filename, string contentType)
        {
            InitRequest(uri, HttpMethod.PUT, null);
            Request.ContentType = contentType;
            Request.PutFilename = filename;
            Request.Expect = true;
            Request.KeepAlive = true;
            return ProcessRequest();
        }

        HttpResponse ProcessRequest(string filename = "")
        {
            var matchingInterceptor = RegisteredInterceptions.FirstOrDefault(i => i.Matches(Request));

            var httpWebRequest = matchingInterceptor != null
                ? new StubbedHttpWebRequest(matchingInterceptor)
                : Request.PrepareRequest();

            var response = new HttpResponse(_decoder);

            response.GetResponse(httpWebRequest, filename, StreamResponse);

            Response = response;

            if (ThrowExceptionOnHttpError && IsHttpError())
            {
                throw new HttpException(Response.StatusCode, Response.StatusDescription);
            }
            return Response;
        }

        public virtual void AddClientCertificates(X509CertificateCollection certificates)
        {
            if(certificates == null || certificates.Count == 0)
                return;

            Request.ClientCertificates.AddRange(certificates);
        }

        bool IsHttpError()
        {
            var num = (int) Response.StatusCode / 100;

            return (num == 4 || num == 5);
        }

        public IHttpRequestInterceptionBuilder OnRequest(Func<HttpRequest,bool> requestPredicate = null)
        {
            var interceptor = new HttpRequestInterception(requestPredicate);

            RegisteredInterceptions.Add(interceptor);

            return interceptor; // so the caller can customize it
        }

        public IHttpRequestInterceptionBuilder OnRequest(HttpMethod method, string url = null)
        {
            var interceptor = new HttpRequestInterception(method, url);

            RegisteredInterceptions.Add(interceptor);

            return interceptor; // so the caller can customize it
        }
    }
}