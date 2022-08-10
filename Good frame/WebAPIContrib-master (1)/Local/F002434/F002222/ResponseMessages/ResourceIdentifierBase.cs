using System;
using System.Net;
using System.Net.Http;

namespace F002222.ResponseMessages
{
    public abstract class ResourceIdentifierBase : HttpResponseMessage
    {
        protected ResourceIdentifierBase(HttpStatusCode httpStatusCode) : base(httpStatusCode)
        {
        }

        protected ResourceIdentifierBase(HttpStatusCode httpStatusCode, Uri resource) : this(httpStatusCode)
        {
            Headers.Location = resource;
        }
    }
}