using ASPMVC1.BusinessLayer;
using ASPMVC1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public class ThreeController : Controller
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
        public ActionResult SavePerson(Person person, string BtPersonSubmit)
        {
            switch (BtPersonSubmit)
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

        public ActionResult SavePerson2(Person person, string BtPersonSubmit2)
        {
            switch (BtPersonSubmit2)
            {
                case "保存Person2":
                    // Request.Form["Name"] 和 ModelBind的person.Name 实现相同的功能
                    new PersonService().SavePerson(person);
                    return RedirectToAction(actionName: "Index");

                case "取消Person2":
                    return RedirectToAction(actionName: "Index");


            }

            return new EmptyResult();
        }

        // html中 post form  时。 控件名称表示传输的对象属性值 
        // 迭代属性名称命名控件
        public string SaveSchool(School school)
        {
            return string.Format("学校名称:{0} 省份:{1} 城市:{2}", school.Name, school.Address.Province, school.Address.City);
        }
    }
}