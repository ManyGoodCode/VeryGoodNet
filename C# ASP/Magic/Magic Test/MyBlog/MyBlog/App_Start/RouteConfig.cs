using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyBlog
{
    public class RouteConfig
    {
        // 这段代码是为MVC应用程序注册了一个路由，这个路由根据url所指的模板去匹配，然后映射到相应的Controller和Action上，
        // 并且默认的Controller和Action是Home和Index,这也是为什么直接访问地址时会自动打开主页面的原因，而/Post/Get/1就代表了Controller是Post、Action
        // 这也是为什么直接访问地址时会自动打开主页面的原因，而/Post/Get/1就代表了Controller是Post、Action
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
