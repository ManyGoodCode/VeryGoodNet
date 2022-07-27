using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EasyHttp.Codecs;
using EasyHttp.Http.Abstractions;
using EasyHttp.Infrastructure;

namespace EasyHttp.Http
{
    public class HttpRequest
    {
        readonly IEncoder encoder;
        HttpRequestCachePolicy cachePolicy;
        bool forceBasicAuth;
        string password;
        string username;
        IHttpWebRequest httpWebRequest;
        CookieContainer cookieContainer;

        public HttpRequest(IEncoder encoder)
        {
            RawHeaders = new Dictionary<string, object>();
            ClientCertificates = new X509CertificateCollection();
            UserAgent = string.Format("EasyHttp HttpClient v{0}",
                                      Assembly.GetAssembly(typeof(HttpClient)).GetName().Version);

            Accept = string.Join(";", HttpContentTypes.TextHtml, HttpContentTypes.ApplicationXml,
                                 HttpContentTypes.ApplicationJson);
            this.encoder = encoder;
            Timeout = 100000; //http://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.timeout.aspx
            AllowAutoRedirect = true;
        }

        public virtual bool DisableAutomaticCompression { get; set; }
        public virtual string Accept { get; set; }
        public virtual string AcceptCharSet { get; set; }
        public virtual string AcceptEncoding { get; set; }
        public virtual string AcceptLanguage { get; set; }
        public virtual bool KeepAlive { get; set; }
        public virtual X509CertificateCollection ClientCertificates { get; set; }
        public virtual string ContentLength { get; private set; }
        public virtual string ContentType { get; set; }
        public virtual string ContentEncoding { get; set; }
        public virtual CookieCollection Cookies { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual bool Expect { get; set; }
        public virtual string From { get; set; }
        public virtual string Host { get; set; }
        public virtual string IfMatch { get; set; }
        public virtual DateTime IfModifiedSince { get; set; }
        public virtual string IfRange { get; set; }
        public virtual int MaxForwards { get; set; }
        public virtual string Referer { get; set; }
        public virtual int Range { get; set; }
        public virtual string UserAgent { get; set; }
        public virtual IDictionary<string, object> RawHeaders { get; private set; }
        public virtual HttpMethod Method { get; set; }
        public virtual object Data { get; set; }
        public virtual string Uri { get; set; }
        public virtual string PutFilename { get; set; }
        public virtual IDictionary<string, object> MultiPartFormData { get; set; }
        public virtual IList<FileData> MultiPartFileData { get; set; }
        public virtual int Timeout { get; set; }
        public virtual Boolean ParametersAsSegments { get; set; }

        public virtual bool ForceBasicAuth
        {
            get { return forceBasicAuth; }
            set { forceBasicAuth = value; }
        }

        public virtual bool PersistCookies { get; set; }
        public virtual bool AllowAutoRedirect { get; set; }

        public virtual void SetBasicAuthentication(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        void SetupHeader()
        {
            if (!PersistCookies || cookieContainer == null)
                cookieContainer = new CookieContainer();

            httpWebRequest.CookieContainer = cookieContainer;
            httpWebRequest.ContentType = ContentType;
            httpWebRequest.Accept = Accept;
            httpWebRequest.Method = Method.ToString();
            httpWebRequest.UserAgent = UserAgent;
            httpWebRequest.Referer = Referer;
            httpWebRequest.CachePolicy = cachePolicy;
            httpWebRequest.KeepAlive = KeepAlive;
            httpWebRequest.AutomaticDecompression = DisableAutomaticCompression
                                                    ? DecompressionMethods.None
                                                    : DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None;

            ServicePointManager.Expect100Continue = Expect;
            ServicePointManager.ServerCertificateValidationCallback = AcceptAllCertifications;

            if (Timeout > 0)
            {
                httpWebRequest.Timeout = Timeout;
            }


            if (Cookies != null)
            {
                httpWebRequest.CookieContainer.Add(Cookies);
            }

            if (IfModifiedSince != DateTime.MinValue)
            {
                httpWebRequest.IfModifiedSince = IfModifiedSince;
            }


            if (Date != DateTime.MinValue)
            {
                httpWebRequest.Date = Date;
            }

            if (!string.IsNullOrEmpty(Host))
            {
                httpWebRequest.Host = Host;
            }

            if (MaxForwards != 0)
            {
                httpWebRequest.MaximumAutomaticRedirections = MaxForwards;
            }

            if (Range != 0)
            {
                httpWebRequest.AddRange(Range);
            }

            SetupAuthentication();

            AddExtraHeader("From", From);
            AddExtraHeader("Accept-CharSet", AcceptCharSet);
            AddExtraHeader("Accept-Encoding", AcceptEncoding);
            AddExtraHeader("Accept-Language", AcceptLanguage);
            AddExtraHeader("If-Match", IfMatch);
            AddExtraHeader("Content-Encoding", ContentEncoding);

            foreach (KeyValuePair<string, object> header in RawHeaders)
            {
                httpWebRequest.Headers.Add(String.Format("{0}: {1}", header.Key, header.Value));
            }
        }

        bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }

        public virtual void AddExtraHeader(string header, object value)
        {
            if (value != null && !RawHeaders.ContainsKey(header))
            {
                RawHeaders.Add(header, value);
            }
        }

        void SetupBody()
        {
            if (Data != null)
            {
                SetupData();
                return;
            }

            if (!string.IsNullOrEmpty(PutFilename))
            {
                SetupPutFilename();
                return;
            }

            if (MultiPartFormData != null || MultiPartFileData != null)
            {
                SetupMultiPartBody();
            }
        }

        void SetupData()
        {
            byte[] bytes = encoder.Encode(Data, ContentType);
            if (bytes.Length > 0)
            {
                httpWebRequest.ContentLength = bytes.Length;
            }

            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
        }

        void SetupPutFilename()
        {
            using (FileStream fileStream = new FileStream(PutFilename, FileMode.Open))
            {
                httpWebRequest.ContentLength = fileStream.Length;
                Stream requestStream = httpWebRequest.GetRequestStream();
                byte[] buffer = new byte[81982];
                int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                while (bytesRead > 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                }

                requestStream.Close();
            }
        }



        void SetupMultiPartBody()
        {
            MultiPartStreamer multiPartStreamer = new MultiPartStreamer(MultiPartFormData, MultiPartFileData);
            httpWebRequest.ContentType = multiPartStreamer.GetContentType();
            long contentLength = multiPartStreamer.GetContentLength();
            if (contentLength > 0)
            {
                httpWebRequest.ContentLength = contentLength;
            }

            multiPartStreamer.StreamMultiPart(httpWebRequest.GetRequestStream());
        }


        public virtual IHttpWebRequest PrepareRequest()
        {
            httpWebRequest = new HttpWebRequestWrapper((HttpWebRequest)WebRequest.Create(Uri));
            httpWebRequest.AllowAutoRedirect = AllowAutoRedirect;
            SetupHeader();
            SetupBody();
            return httpWebRequest;
        }

        void SetupClientCertificates()
        {
            if (ClientCertificates == null || ClientCertificates.Count == 0)
                return;
            httpWebRequest.ClientCertificates.AddRange(ClientCertificates);
        }

        void SetupAuthentication()
        {
            SetupClientCertificates();
            if (forceBasicAuth)
            {
                string authInfo = username + ":" + password;
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                httpWebRequest.Headers["Authorization"] = "Basic " + authInfo;
            }
            else
            {
                NetworkCredential networkCredential = new NetworkCredential(username, password);
                httpWebRequest.Credentials = networkCredential;
            }
        }


        public virtual void SetCacheControlToNoCache()
        {
            cachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
        }

        public virtual void SetCacheControlWithMaxAge(TimeSpan maxAge)
        {
            cachePolicy = new HttpRequestCachePolicy(HttpCacheAgeControl.MaxAge, maxAge);
        }

        public virtual void SetCacheControlWithMaxAgeAndMaxStale(TimeSpan maxAge, TimeSpan maxStale)
        {
            cachePolicy = new HttpRequestCachePolicy(HttpCacheAgeControl.MaxAgeAndMaxStale, maxAge, maxStale);
        }

        public virtual void SetCacheControlWithMaxAgeAndMinFresh(TimeSpan maxAge, TimeSpan minFresh)
        {
            cachePolicy = new HttpRequestCachePolicy(HttpCacheAgeControl.MaxAgeAndMinFresh, maxAge, minFresh);
        }
    }
}