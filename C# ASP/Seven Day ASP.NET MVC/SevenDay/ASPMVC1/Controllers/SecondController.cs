using ASPMVC1.Models;
using ASPMVC1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public class SecondController : System.Web.Mvc.Controller
    {
        // ViewData传值
        // 1. 视图一定要在 Views ->  Second 文件夹中，不同控制器的命名可以相同。返回的名称要写在 return View(viewName: "View1");
        // 2. 浏览器按F12可以查看具体显示情况
        public ActionResult GetViewData()
        {
            Employee e = new Employee()
            {
                FirstName = "Wu",
                LastName = " Xiao",
                Salary = 2000
            };

            ViewData["Employee"] = e;
            return View(viewName: "View1");
        }

        // ViewBag传值
        public ActionResult GetViewBag()
        {
            Employee e = new Employee()
            {
                FirstName = "Li",
                LastName = " Xiao",
                Salary = 1234
            };

            ViewBag.Employee = e;
            return View(viewName: "View2");
        }

        // 强类型视图 Model 与 ViewModel的关系
        // ！！！！！！！！！！！！！  Model和ViewModel 是互相独立的，Controller将根据Model对象创建并初始化ViewModel对象。
        // http://localhost:62939/Second/StrongView?salary=1030 渲染成绿色
        // http://localhost:62939/Second/StrongView?salary=2000 渲染成黄色
        public ActionResult StrongView(int salary)
        {
            Employee e = new Employee()
            {
                FirstName = "Li",
                LastName = " Xiao",
            };

            e.Salary = salary;
            return View(viewName: "View3", model: e);
        }

        // http://localhost:62939/Second/StrongViewLst
        // 将ViewModel直接传入html 渲染 表格
        public ActionResult StrongViewLst()
        {
            List<StudentViewModel> model = new List<StudentViewModel>()
            {
                new StudentViewModel(){Name  ="aaa",Score = 50},
                new StudentViewModel(){Name  ="bbb",Score = 60},
                new StudentViewModel(){Name  ="ccc",Score = 40},
                new StudentViewModel(){Name  ="ddd",Score = 80},
                new StudentViewModel(){Name  ="eee",Score = 70},

            };

            model.ForEach(m => m.ScoreColor = m.Score >= 60 ? "green" : "yellow");
            StudentListViewModel viewModel = new StudentListViewModel()
            {
                Students = model,
                LoginName = "Admin"
            };

            return View(viewName: "View4", model: viewModel);
        }
    }
}