using ASPMVC1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public partial class ThreeController : System.Web.Mvc.Controller
    {
        public ActionResult ModelBind()
        {
            return View(viewName: "ModelBind");
        }

        // Post 表格绑定的方法1 当Post提交的Form 和 函数的参数名称一样时【但是与对象属性不一致】
        public string Bind1(string ame, int alary)
        {
            Person p = new Person()
            {
                Name = ame,
                Salary = alary
            };

            return string.Format("Bind1 Name:{0}  Salary:{1}", p.Name, p.Salary);
        }


        // Post 表格绑定的方法2 当Post提交的Form 和 函数的参数名称一样时【但是与对象属性不一致】
        public string Bind2()
        {
            string name = Request.Form["ame"];
            string salary = Request.Form["alary"];
            return string.Format("Bind2 Name:{0}  Salary:{1}", name, salary);
        }

        // 自适配 DefaultModelBinder，将提交的字段映射到对象
        public string Bind3([ModelBinder(typeof(PersonModelBinder))]Person p)
        {
            return string.Format("Bind3 Name:{0}  Salary:{1}", p.Name, p.Salary);
        }
    }

    public class PersonModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Person p = new Person();
            p.Name = controllerContext.HttpContext.Request.Form["ame"];
            p.Salary = int.Parse(controllerContext.HttpContext.Request.Form["alary"]);
            return p;
        }
    }
}