using ASPMVC1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public class SecondController : System.Web.Mvc.Controller
    {
        // 1. 视图一定要在 Views ->  Second 文件夹中，不同控制器的命名可以相同。返回的名称要写在 return View(viewName: "View1");
        // 2. 浏览器按F12可以查看具体显示情况
        public ActionResult GetViewData()
        {
            Employee e = new Employee()
            {
                FirstName = "Wu",
                LastName = " Xiao",
                Salary = 2000
            };

            ViewData["Employee"] = e;
            return View(viewName: "View1");
        }


        public ActionResult GetViewBag()
        {
            Employee e = new Employee()
            {
                FirstName = "Li",
                LastName = " Xiao",
                Salary = 1234
            };

            ViewBag.Employee = e;
            return View(viewName: "View2");
        }
    }
}