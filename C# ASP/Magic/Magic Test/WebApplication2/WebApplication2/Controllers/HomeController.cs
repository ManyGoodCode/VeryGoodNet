using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Business;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        SerialPortManager serialPortManager = SerialPortManager.GetInstance();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Rev()
        {
            string rev = serialPortManager.Send("Your application description page." + DateTime.Now.ToString("HH:mm:ss fff"));
            return Content(rev);
        }
    }
}