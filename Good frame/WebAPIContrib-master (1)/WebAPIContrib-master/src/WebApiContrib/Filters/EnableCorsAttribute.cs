using System.Linq;
using System.Web.Http.Filters;

namespace WebApiContrib.Filters
{    
    public class EnableCorsAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private const string origin = "Origin";
        private const string accessControlAllowOrigin = "Access-Control-Allow-Origin";

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Request.Headers.Contains(origin))
            {
                string originHeader = actionExecutedContext.Request.Headers.GetValues(origin).FirstOrDefault();
                if (!string.IsNullOrEmpty(originHeader))
                {
                    actionExecutedContext.Response.Headers.Add(accessControlAllowOrigin, originHeader);
                }
            }
        }
    }
}
