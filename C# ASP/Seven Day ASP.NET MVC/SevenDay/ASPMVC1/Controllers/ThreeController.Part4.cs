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
        public ActionResult LoadJavaScript()
        {
            return View(viewName: "LoadJavaScript");
        }

        public  string NowJavaScriptURL(Person p)
        {
            return string.Format("NowJavaScriptURL {0}{1}", p.Name, p.Salary);
        }

        public string ChangeJavaScriptURL(Person p)
        {
            return string.Format("ChangeJavaScriptURL {0}{1}", p.Name, p.Salary);
        }
    }
}