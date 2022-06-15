using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    //打开 Web.config文件，在System.Web部分，找到Authentication的子标签。如果不存在此标签，就在文件中添加Authentication标签。
    // 设置Authentication的Mode为Forms，loginurl设置为”Login”方法的URL.
    //  <authentication mode = "Forms" >
    //  <forms loginUrl="~/Authentication/Login"></forms>
    //  </authentication>
    public partial class FourController : Controller
    {
        public ActionResult Login()
        {
            return View(viewName: "Login");
        }

        //  Html.BeginForm(actionName: "DoLogin", controllerName: "Four", method: FormMethod.Post) 转换为html代码<form action="/Authentication/DoLogin" method="post"> --->
        // Html.TextAreaFor(expression: x => x.UserName) 转换为html代码<input id="UserName" name="UserName" type="text" value="" --->
        public ActionResult DoLogin()
        {
            return Content("DoLogin");
        }

        // 让Action 方法更安全; 在Index action 方法中添加认证属性 [Authorize].
        // 运行测试，输入 FourController 的 Index action的URL：“http://localhost:62939/Four/TestAuthentication1”
        [Authorize]
        public ActionResult TestAuthentication1()
        {
            return Content("TestAuthentication1");
        }

        // 和上面没有加特性的进行比较
        public ActionResult TestAuthentication2()
        {
            return Content("TestAuthentication2");
        }
    }

    public class UserDetails
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}