using System.Web.Http;

namespace WebApiContrib.Testing
{
    public class DummyController : System.Web.Http.ApiController
    {
        public string Get()
        {
            return "OK";
        }
    }
}
