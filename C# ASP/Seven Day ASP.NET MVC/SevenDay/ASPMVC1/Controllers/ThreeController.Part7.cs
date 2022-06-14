using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public partial class ThreeController : Controller
    {
        public ActionResult Verify()
        {
            return View(viewName: "Verify");
        }

        // ModelState 服务器有效性验证
        public string VerifyModelBind(VerifyModel model)
        {
            if (!ModelState.IsValid)
            {
                return string.Empty;
            }
            else
            {
                return string.Format("Key:{0} Text:{1}", model.Key, model.Text);
            }
        }
    }

    public class VerifyModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        public int Key { get; set; }


        [System.ComponentModel.DataAnnotations.StringLength(maximumLength: 1)]
        public string Text { get; set; }
    }
}