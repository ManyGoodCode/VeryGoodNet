using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public class FourController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // 无效验证把默认值返回
        public ActionResult CallBack(FourEntity entity)
        {
            if (ModelState.IsValid)
            {
                return View(viewName: "CallBack", model: new FourEntity()
                {
                    Name = "????",
                    PassWord = "*****"
                }) ;
            }
            else
            {
                return View(viewName: "Index");
            }
        }
    }

    public class FourEntity
    {
        [StringLength(5)]
        public string Name { get; set; }

        [StringLength(6)]
        public string PassWord { get; set; }
    }
}