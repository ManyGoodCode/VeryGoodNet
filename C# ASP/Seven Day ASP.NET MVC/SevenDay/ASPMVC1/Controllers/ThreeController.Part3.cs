using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public partial class ThreeController : Controller
    {
        public ActionResult HideCancelFom()
        {
            return View(viewName: "HideCancel");
        }

        [HttpGet]
        public string GetCancelClick()
        {
            return "this is Hide Cancel Click";
        }
    }
}