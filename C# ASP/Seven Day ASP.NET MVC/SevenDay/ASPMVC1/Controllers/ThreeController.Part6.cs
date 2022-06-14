using ASPMVC1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public partial class ThreeController : System.Web.Mvc.Controller
    {
        public ActionResult ModelBind()
        {
            return View(viewName: "ModelBind");
        }

        // Post 表格绑定的方法1
        public string Bind1(Person p)
        {
            return string.Format("Bind1 Name:{0}  Salary:{1}", p.Name, p.Salary);
        }


        // Post 表格绑定的方法2
        public string Bind2()
        {
            string name = Request.Form["Name"];
            string salary = Request.Form["Salary"];
            return string.Format("Bind2 Name:{0}  Salary:{1}", name, salary);
        }
    }
}