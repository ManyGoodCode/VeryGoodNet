using ASPMVC1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public partial class ThreeController : Controller
    {
        public ActionResult PrintSchool()
        {
            return View(viewName: "PrintSchool");
        }

        // html中 post form  时。 控件名称表示传输的对象属性值 
        // 迭代属性名称命名控件
        public string PrintShoolInfo(School school)
        {
            return string.Format("学校名称:{0} 省份:{1} 城市:{2}", school.Name, school.Address.Province, school.Address.City);
        }
    }
}