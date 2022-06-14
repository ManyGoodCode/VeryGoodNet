using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public partial class ThreeController : System.Web.Mvc.Controller
    {
        public ActionResult ResetView()
        {
            return View(viewName: "ResetView"); 
        }
    }
}