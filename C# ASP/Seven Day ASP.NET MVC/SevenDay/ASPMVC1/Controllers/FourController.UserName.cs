using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ASPMVC1.Controllers
{
    public partial class FourController : Controller
    {
        // 在界面右边显示用户名称，指示当前登录的用户名
        [Authorize]
        public ActionResult RightUserName()
        {
            RightUser user = new RightUser()
            {
                Name = "Wu,Nicky"
            };

            return View(viewName: "RightUserName", model: user);
        }


        // 退出登录的Cookie表示当需要访问TestAuthentication1特性授权的资源时会让你登录
        public ActionResult LoginOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction(actionName: "TestAuthentication1");
        }
    }

    public class RightUser
    {
        public string Name { get; set; }
    }
}