using ASPMVC1.BusinessLayer;
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
        public ActionResult AddDataBasePerson()
        {
            return View(viewName: "AddDataBasePerson");
        }

        public ActionResult SaveDataBasePerson(Person person, string BtSubmit)
        {
            switch (BtSubmit)
            {
                case "保存保存数据库Person":
                    // Request.Form["Name"] 和 ModelBind的person.Name 实现相同的功能
                    new PersonService().SavePerson(person);
                    return RedirectToAction(actionName: "Index");

                case "取消Person":
                    return RedirectToAction(actionName: "Index");


            }

            return new EmptyResult();
        }
    }
}