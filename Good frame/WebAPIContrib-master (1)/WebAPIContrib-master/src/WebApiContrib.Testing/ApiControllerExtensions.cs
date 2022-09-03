using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace WebApiContrib.Testing
{
    public static class ApiControllerExtensions
    {
        public static void ConfigureForTesting(
            this System.Web.Http.ApiController controller,
            System.Net.Http.HttpMethod method, 
            string uri, 
            string routeName = null,
            System.Web.Http.Routing.IHttpRoute route = null)
        {
            System.Net.Http.HttpRequestMessage request = new HttpRequestMessage(method, uri);
            ConfigureForTesting(controller, request, routeName, route);
        }

        public static void ConfigureForTesting(
            this System.Web.Http.ApiController controller,
            System.Net.Http.HttpRequestMessage request, 
            string routeName = null,
            System.Web.Http.Routing.IHttpRoute route = null)
        {
            HttpConfiguration config = new HttpConfiguration();
            controller.Configuration = config;
            if (routeName != null && route != null)
                config.Routes.Add(routeName, route);
            else
                route = config.Routes.MapHttpRoute("DefaultApi", "{controller}/{id}", new { id = RouteParameter.Optional });

            string controllerTypeName = controller.GetType().Name;
            string controllerName = controllerTypeName.Substring(0, controllerTypeName.IndexOf("Controller")).ToLower();
            System.Web.Http.Routing.HttpRouteData routeData = new HttpRouteData(
                route, new HttpRouteValueDictionary 
                {
                    { "controller", controllerName } 
                });

            request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData;
            controller.ControllerContext = new System.Web.Http.Controllers.HttpControllerContext(config, routeData, request);
            controller.ControllerContext.ControllerDescriptor = new HttpControllerDescriptor(config, controllerName, controller.GetType());
        }
    }
}
