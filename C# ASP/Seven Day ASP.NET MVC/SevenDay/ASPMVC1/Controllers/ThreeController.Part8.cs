using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public partial class ThreeController : Controller
    {
        public ActionResult VerifyWithErrorMessage()
        {
            return View(viewName: "VerifyWithErrorMessage");
        }

        // ModelState 服务器有效性验证
        public string VerifyWithErrorMessageModelBind(VerifyModelWithErrorMessage model)
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
    public class VerifyModelWithErrorMessage
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "key must has value")]
        public int Key { get; set; }


        [System.ComponentModel.DataAnnotations.StringLength(maximumLength: 1,ErrorMessage = "Text max length = 1")]
        public string Text { get; set; }
    }

}