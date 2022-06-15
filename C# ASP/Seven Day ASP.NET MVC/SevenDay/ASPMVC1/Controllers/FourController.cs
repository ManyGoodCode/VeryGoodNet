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


        // 成功就返回OK，没成功就把验证通过的数传回，看着像没改
        public ActionResult CallBack(FourEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return View(viewName: "CallBack", model: new FourEntity()
                {
                    Name = entity.Name,
                    PassWord = entity.PassWord
                }) ;
            }
            else
            {
                return Content("OK");
            }
        }
    }

    public class FourEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string PassWord { get; set; }
    }
}