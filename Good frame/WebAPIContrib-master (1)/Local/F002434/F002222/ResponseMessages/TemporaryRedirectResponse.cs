using System;
using System.Net;

namespace F002222.ResponseMessages
{
    public class TemporaryRedirectResponse : ResourceIdentifierBase
    {
        public TemporaryRedirectResponse() : base(HttpStatusCode.TemporaryRedirect)
        {
        }

        public TemporaryRedirectResponse(Uri resource) : base(HttpStatusCode.TemporaryRedirect, resource)
        {
        }
    }
}