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
        public ActionResult StrongViewLst()
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

            return View(viewName: "View1", model: viewModel);
        }
    }
}