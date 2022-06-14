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
        // http://localhost:62939/Three/StrongViewLst
        // 本地数据库继承软件需要引用 EntityFramework
        public ActionResult Index()
        {
            List<PersonModel> model = new List<PersonModel>();
            List<Person> persons = new PersonService().GetPersons();

            persons.ForEach(p =>
            {
                model.Add(new PersonModel()
                {
                    Name = p.Name,
                    Salary = p.Salary,
                    Color = p.Salary < 1000 ? "green" : "yellow"
                });
            });
            PersonLstView viewModel = new PersonLstView()
            {
                PersonModels = model,
                LoginUser = "Admin"
            };

            return View(viewName: "Index", model: viewModel);
        }

        public ActionResult AddPerson()
        {
            return View(viewName: "AddPerson");
        }


        // html中 post form  时。1. action 代表执行的控制器方法;2. 控件名称表示传输的对象属性值 
        // 获取Submit的名称:RedirectToAction跳到当前控制器的位置；EmptyResult为空
        public ActionResult PrintPerson(Person person, string BtSubmit)
        {
            switch (BtSubmit)
            {
                case "保存Person":
                    // Request.Form["Name"] 和 ModelBind的person.Name 实现相同的功能
                    string name = Request.Form["Name"];
                    return Content(content: string.Format("{0}|{1}", name, person.Salary));

                case "取消Person":
                    return RedirectToAction(actionName: "Index");


            }

            return new EmptyResult();
        }
    }
}