using System;
using System.Net;

namespace F002222.ResponseMessages
{
    public class MovedPermanentlyResponse : ResourceIdentifierBase
    {
        public MovedPermanentlyResponse() : base(HttpStatusCode.MovedPermanently)
        {
        }

        public MovedPermanentlyResponse(Uri resource) : base(HttpStatusCode.MovedPermanently, resource)
        {
        }
    }
}