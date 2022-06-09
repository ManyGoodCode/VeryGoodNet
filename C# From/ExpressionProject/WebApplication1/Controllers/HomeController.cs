using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        // 获得中国省份、直辖市、地区和与之对应的ID
        // 输入参数：无，返回数据：一维字符串数组。
        public const string ProviceIDURL = "http://ws.webxml.com.cn/WebServices/WeatherWS.asmx/getRegionProvince";

        // 获得支持的省市（地区）和分类电视名称 String()
        // 输入参数：无；返回数据：一个一维字符串数组 String()，结构为：省市和分类电视ID@省市和分类电视名称@所属地区。
        public const string ProviceTVDRL = "http://ws.webxml.com.cn/webservices/ChinaTVprogramWebService.asmx/getAreaString";

        public readonly static Dictionary<string, string> Cache = new Dictionary<string, string>();

        public HomeController()
        {
            string body = string.Empty;
            if (!Cache.Keys.Contains(ProviceIDURL))
            {
                body = GetURL(ProviceIDURL);
                Cache.Add(key: ProviceIDURL, value: body);
            }

            if (!Cache.Keys.Contains(ProviceTVDRL))
            {
                body = GetURL(ProviceTVDRL);
                Cache.Add(key: ProviceTVDRL, value: body);
            }
        }

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

        // http://localhost:65248/Home/GetProviceID
        [HttpGet]
        public ActionResult GetProviceID(string nameContain)
        {
            string body = string.Empty;
            if (!Cache.TryGetValue(ProviceIDURL, out body) || string.IsNullOrWhiteSpace(body))
            {
                body = GetURL(ProviceIDURL);
                Cache.Add(key: ProviceIDURL, value: body);
            }

            return Content(body);
        }

        // 不带参数可直接返回全部数据: http://localhost:65248/Home/GetProviceTV
        // 带参数可返回过滤数据: http://localhost:65248/Home/GetProviceTV?nameContain=a
        [HttpGet]
        public ActionResult GetProviceTV(string nameContain)
        {
            string body = string.Empty;
            if (!Cache.TryGetValue(ProviceTVDRL, out body) || string.IsNullOrWhiteSpace(body))
            {
                body = GetURL(ProviceIDURL);
                Cache.Add(key: ProviceIDURL, value: body);
            }

            List<string> items = body.Split('\n').ToList();
            int length = items.Count;
            items.RemoveAt(length - 1);
            items.RemoveAt(1);
            items.RemoveAt(0);
            for (int i = 0; i < items.Count; i++)
            {
                items[i] = items[i].Replace("\r", string.Empty)
                      .Replace("<string>", string.Empty)
                      .Replace("</string>", string.Empty);

            }

            if (!string.IsNullOrWhiteSpace(nameContain))
            {
                items = items.FindAll(i => i.Contains(nameContain));
            }

            body = string.Join("\r\n", items);
            return Content(body);
        }

        private string GetURL(string url)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(url);
            System.Net.WebResponse myResponse = request.GetResponse();
            string body = string.Empty;
            using (Stream stream = myResponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    body = reader.ReadToEnd();
                }
            }

            myResponse.Close();

            return body;
        }
    }
}