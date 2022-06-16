using ASPMVC1.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ASPMVC1.Controllers
{
    public class FiveController : Controller
    {
        // 显示分布视图 ，将模型对应的值进行传输
        // 分布视图放在 Share文件夹里面,创建的时候选择Partial.其无后台程序
        // Footer.cshtml,
        public ActionResult Index()
        {
            FiveMainViewModel viewModel = new FiveMainViewModel()
            {
                Text = "主界面",
                Foot = new FooterViewModel() {
                    CompanyName = "HonyWell",
                    Year = "1999"
                }
            };

            return View(viewName: "IndexView",model: viewModel);
        }
    }
}