﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Business;
using WebApplication2.ViewModel;

namespace WebApplication2.Controllers
{
    public class PageViewController : Controller
    {

        public ActionResult Get(int id)
        {
            DocumentManager manager = new DocumentManager();
            Dcument doc = manager.GetDocumentByID(id);

            ContentViewModel viewDTO = new ContentViewModel()
            {
                Title = doc.Title,
                Content = doc.Content,
                UserName = doc.UserName
            };

            return View(model: viewDTO);
        }

        public ActionResult Index()
        {
            DocumentManager manager = new DocumentManager();
            List<Dcument> docs = manager.GetAllDocument();
            DocumentListView viewDTO = new DocumentListView()
            {
                Count = docs.Count,
                PageCount = 1,
                CurrentPage = 1,
                Docs = docs
            };

            return View(model: viewDTO);
        }

        public ActionResult GetDocumentJson()
        {
            DocumentManager manager = new DocumentManager();
            List<Dcument> docs = manager.GetAllDocument();
            JsonResult result = new JsonResult();
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            result.Data = docs;
            return result;
        }
    }
}