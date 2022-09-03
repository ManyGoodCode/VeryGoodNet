using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiContrib.Testing
{
    public class FakeHandler : DelegatingHandler
    {
        private readonly Func<System.Net.Http.HttpRequestMessage, System.Net.Http.HttpResponseMessage> f;

        public FakeHandler(Func<System.Net.Http.HttpRequestMessage, System.Net.Http.HttpResponseMessage> f)
        {
            this.f = f;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            System.Net.Http.HttpRequestMessage request,
            System.Threading.CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() => f(request));
        }
    }
}
