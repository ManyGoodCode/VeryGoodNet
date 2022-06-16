using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public partial class FourController : Controller
    {
        public ActionResult LoadJsView()
        {
            return View(viewName: "LoadJsView");
        }


        public ActionResult NoImportJs(ImportJsEnitity enitity)
        {
            if (!ModelState.IsValid)
            {
                return Content("OK");
            }
            else
            {
                ModelState.AddModelError(key: "CredentialError", errorMessage: "Invalid Username");
                return View(viewName: "NoImportJs");
            }
        }
    }

    public class ImportJsEnitity
    {
        [StringLength(7, MinimumLength = 2, ErrorMessage = "UserName length should be between 2 and 7")]
        public string UserName { get; set; }

    }
}