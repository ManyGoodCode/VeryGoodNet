using System;
using System.IO;
using System.Net;
using System.Text;
using EasyHttp.Codecs;
using EasyHttp.Configuration;
using EasyHttp.Http.Abstractions;

namespace EasyHttp.Http
{
    public class HttpResponse
    {
        readonly EasyHttp.Codecs.IDecoder decoder;
        EasyHttp.Http.Abstractions.IHttpWebResponse response;
        public virtual string CharacterSet { get; private set; }
        public virtual string ContentType { get; private set; }
        public virtual HttpStatusCode StatusCode { get; private set; }
        public virtual string StatusDescription { get; private set; }
        public virtual CookieCollection Cookies { get; private set; }
        public virtual int Age { get; private set; }
        public virtual HttpMethod[] Allow { get; private set; }
        public virtual CacheControl CacheControl { get; private set; }
        public virtual string ContentEncoding { get; private set; }
        public virtual string ContentLanguage { get; private set; }
        public virtual long ContentLength { get; private set; }
        public virtual string ContentLocation { get; private set; }
        public virtual string ContentDisposition { get; private set; }
        public virtual DateTime Date { get; private set; }
        public virtual string ETag { get; private set; }
        public virtual DateTime Expires { get; private set; }
        public virtual DateTime LastModified { get; private set; }
        public virtual string Location { get; private set; }
        public virtual CacheControl Pragma { get; private set; }
        public virtual string Server { get; private set; }
        public virtual WebHeaderCollection RawHeaders { get; private set; }
        public virtual Stream ResponseStream
        {
            get { return response.GetResponseStream(); }
        }

        public virtual dynamic DynamicBody
        {
            get { return decoder.DecodeToDynamic(RawText, ContentType); }
        }

        public virtual string RawText { get; private set; }

        public virtual T StaticBody<T>(string overrideContentType = null)
        {
            if (overrideContentType != null)
            {
                return decoder.DecodeToStatic<T>(RawText, overrideContentType);
            }

            return decoder.DecodeToStatic<T>(RawText, ContentType);
        }

        public HttpResponse()
            : this(null)
        {
        }

        public HttpResponse(IDecoder decoder)
        {
            this.decoder = decoder ?? new DefaultEncoderDecoderConfiguration().GetDecoder();
        }



        public virtual void GetResponse(IHttpWebRequest request, string filename, bool streamResponse)
        {
            try
            {
                response = request.GetResponse();
            }
            catch (WebException webException)
            {
                if (webException.Response == null)
                {
                    throw;
                }

                response = new HttpWebResponseWrapper((HttpWebResponse)webException.Response);
            }

            GetHeaders();
            if (streamResponse)
                return;
            using (Stream stream = response.GetResponseStream())
            {
                if (stream == null)
                    return;
                if (!string.IsNullOrEmpty(filename))
                {
                    using (FileStream filestream = new FileStream(filename, FileMode.CreateNew))
                    {
                        int count;
                        byte[] buffer = new byte[8192];
                        while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            filestream.Write(buffer, 0, count);
                        }
                    }
                }
                else
                {
                    Encoding encoding = string.IsNullOrEmpty(CharacterSet) ? Encoding.UTF8 : Encoding.GetEncoding(CharacterSet);
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        RawText = reader.ReadToEnd();
                    }
                }
            }
        }

        void GetHeaders()
        {
            CharacterSet = response.CharacterSet;
            ContentType = response.ContentType;
            StatusCode = response.StatusCode;
            StatusDescription = response.StatusDescription;
            Cookies = response.Cookies;
            ContentEncoding = response.ContentEncoding;
            ContentLength = response.ContentLength;
            Date = DateTime.Now;
            LastModified = response.LastModified;
            Server = response.Server;
            if (!string.IsNullOrEmpty(GetHeader("Age")))
            {
                Age = Convert.ToInt32(GetHeader("Age"));
            }

            ContentLanguage = GetHeader("Content-Language");
            ContentLocation = GetHeader("Content-Location");
            ContentDisposition = GetHeader("Content-Disposition");
            ETag = GetHeader("ETag");
            Location = GetHeader("Location");

            if (!string.IsNullOrEmpty(GetHeader("Expires")))
            {
                DateTime expires;
                if (DateTime.TryParse(GetHeader("Expires"), out expires))
                {
                    Expires = expires;
                }
            }

            RawHeaders = response.Headers;
        }

        string GetHeader(string header)
        {
            string headerValue = response.GetResponseHeader(header);
            return headerValue.Replace("\"", "");
        }
    }
}