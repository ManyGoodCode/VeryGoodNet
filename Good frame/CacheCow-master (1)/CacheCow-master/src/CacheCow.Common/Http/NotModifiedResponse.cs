﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;

namespace CacheCow.Common.Http
{
    /// <summary>
    /// NotModifiedResponse 继承 HttpResponseMessage。构造器设置属性 HttpRequestMessage，CacheControlHeaderValue
    /// </summary>
	public class NotModifiedResponse : HttpResponseMessage
	{
        public NotModifiedResponse(
            HttpRequestMessage request,
            CacheControlHeaderValue cacheControlHeaderValue)
			: this(request, cacheControlHeaderValue, null)
		{
		}


		public NotModifiedResponse(
            HttpRequestMessage request,
            CacheControlHeaderValue cacheControlHeaderValue,
            EntityTagHeaderValue etag)
			: base(HttpStatusCode.NotModified)
		{
			if(etag!=null)
				this.Headers.ETag = etag;
		    this.Headers.CacheControl = cacheControlHeaderValue;
			this.RequestMessage = request;
		}
	}
}
