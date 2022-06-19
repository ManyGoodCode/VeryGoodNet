using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyBlog.Controllers
{
    public class PostController : Controller
    {
        private static List<Post> Posts = new List<Post>()
        {
            new Post(){ ID =1,Title ="文章1", Author = "Nicky,Wu", Content = "文章1的内容",CreateDate = DateTime.Now,ModifyDate= DateTime.Now },
            new Post(){ ID =2,Title ="文章2", Author = "Nicky,Wu", Content = "文章2的内容",CreateDate = DateTime.Now,ModifyDate= DateTime.Now },
            new Post(){ ID =3,Title ="文章3", Author = "Nicky,Wu", Content = "文章3的内容",CreateDate = DateTime.Now,ModifyDate= DateTime.Now },
            new Post(){ ID =4,Title ="文章4", Author = "Nicky,Wu", Content = "文章4的内容",CreateDate = DateTime.Now,ModifyDate= DateTime.Now },
            new Post(){ ID =5,Title ="文章5", Author = "Nicky,Wu", Content = "文章5的内容",CreateDate = DateTime.Now,ModifyDate= DateTime.Now }
        };

        public ActionResult Index()
        {
            ViewBag.Posts = Posts;
            return View();
        }

        public ActionResult Get(int id)
        {
            Post post = Posts.Where(p => p.ID == id).FirstOrDefault();
            if (post == null)
            {
                return HttpNotFound();
            }

            ViewBag.Post = post;
            return View();
        }
    }
}