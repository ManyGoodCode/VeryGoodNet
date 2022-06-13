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

            persons.ForEach(p => {
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
            return View(viewName:"AddPerson");
        }

        // html中 post form  时。1. action 代表执行的控制器方法;2. 控件名称表示传输的对象属性值 
        public string SavePerson(Person person)
        {
            return string.Format("{0}|{1}", person.Name, person.Salary);
        }
    }
}