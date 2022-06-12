using ASPMVC1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public class TestController : Controller
    {
        // http://localhost:62939/Test/GetString
        // 浏览器可以通过F12获取获得的html数据
        public string GetString()
        {
            return "Hello World is old now.It's time for wassup bro;";
        }

        // http://localhost:62939/Test/GetCustomer
        // 1. 当返回"Customer"这样的类似的对象时，将返回ToString()结果 ASPMVC1.Models.Customer
        // 2. 重写  ToString() 为 this.CustomerName+"|"+this.Address 正常返回
        public ASPMVC1.Models.Customer GetCustomer()
        {
            Customer c = new Customer()
            {
                Name = "客户1",
                Address = "Address1"
            };

            return c;
        }

        // 因为用 NonAction 特性标记 ，提示无法找到资源
        [NonAction]
        public string NoActionMethod()
        {
            return "NoActionMethod";
        }

        // 1. 显示 html界面
        // 2 
        //    2.1. View是放置在特定目录下，以'ControlName'命名的，并且放在View文件夹内
        //    2.2. 将View文件夹放在Shared文件夹中所有的Controller都可用
        // 3. ActionResult 是一个抽象类； ViewResult是多级子节点 ； ContentResult
        public System.Web.Mvc.ActionResult GetView(int? index)
        {
            if (!index.HasValue)
                return View("Test");
            else
                return View("ShareTest");
        }
    }
}