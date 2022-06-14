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
        // 错误信息返回客户端 并且通过 @Html.ValidationMessage("Key") 获取，【界面被重新加载 需要注意与之前不同】

        public ActionResult VerifyWithErrorMessageModelBind(VerifyModelWithErrorMessage model)
        {
            if (!ModelState.IsValid)
            {
                return View(viewName: "VerifyErrorView");
            }
            else
            {
                return RedirectToAction(actionName: "Index");
            }
        }
    }
    public class VerifyModelWithErrorMessage
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Enter Key")]
        public int Key { get; set; }


        [System.ComponentModel.DataAnnotations.StringLength(maximumLength: 1,ErrorMessage = "Text max length = 1")]
        public string Text { get; set; }
    }

}