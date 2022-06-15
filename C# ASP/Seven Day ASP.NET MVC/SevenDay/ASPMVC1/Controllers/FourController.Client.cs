using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    // 客户端验证 参考文件 Scripts文件夹下  Validations.js
    // 参考View -> Four 文件夹下 FourValidation.chtml
    public partial class FourController : Controller
    {
        public ActionResult FourValidation()
        {
            return View(viewName: "FourValidation");
        }
    }
}