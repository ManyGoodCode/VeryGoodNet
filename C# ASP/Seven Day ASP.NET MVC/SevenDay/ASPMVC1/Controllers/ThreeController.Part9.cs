using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    // 定义服务器端验证器
    public partial class ThreeController : Controller
    {
        public ActionResult CustomVerify()
        {
            return View(viewName: "CustomVerify");
        }

        // ModelState 服务器有效性验证 
        // 错误信息返回客户端 并且通过 @Html.ValidationMessage("Key") 获取，【界面被重新加载 需要注意与之前不同】

        public ActionResult CustomVerifyBack(CustomVerify model)
        {
            if (!ModelState.IsValid)
            {
                return View(viewName: "CustomVerifyBack");
            }
            else
            {
                return RedirectToAction(actionName: "Index");
            }
        }
    }

    public class CustomVerify
    {
        [NullOrErrorContainValidation]
        public string Key { get; set; }


        [NullOrErrorContainValidation]
        public string Text { get; set; }
    }

    public class NullOrErrorContainValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // 检查对象为空
            if (value == null)
            {
                return new ValidationResult(errorMessage:"this object can‘t be  null");
            }
            if (value.ToString().Contains("@"))
            {
                return new ValidationResult(errorMessage: "this object can‘t contain  @");
            }

            return ValidationResult.Success;
        }
    }
}