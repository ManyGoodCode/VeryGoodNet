using System.Net.Http;
using System.Web.Http;

namespace WebApiContrib.Testing
{
    public static class TestFactory
    {
        public static System.Net.Http.HttpRequestMessage GetDefaultRequest()
        {
            System.Net.Http.HttpRequestMessage request = new System.Net.Http.HttpRequestMessage(
               method: HttpMethod.Get, 
               requestUri:  "http://test/");
            return request;
        }

        public static System.Web.Http.HttpConfiguration GetDefaultConfiguration()
        {
            System.Web.Http.HttpConfiguration httpConfig = new System.Web.Http.HttpConfiguration();
            return httpConfig;
        }

        public static System.Web.Http.HttpServer GetDefaultServer()
        {
            return new System.Web.Http.HttpServer(
                configuration: GetDefaultConfiguration(), 
                dispatcher: new FakeHandler(req => new System.Net.Http.HttpResponseMessage()));
        }
    }
}
