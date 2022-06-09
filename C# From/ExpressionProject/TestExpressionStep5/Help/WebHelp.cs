using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestExpressionStep5
{
    public class WebHelp
    {
        // 获得中国省份、直辖市、地区和与之对应的ID
        // 输入参数：无，返回数据：一维字符串数组。
        public const string ProviceIDURL = "http://ws.webxml.com.cn/WebServices/WeatherWS.asmx/getRegionProvince";

        // 获得支持的省市（地区）和分类电视名称 String()
        // 输入参数：无；返回数据：一个一维字符串数组 String()，结构为：省市和分类电视ID@省市和分类电视名称@所属地区。
        public const string ProviceTVDRL = "http://ws.webxml.com.cn/webservices/ChinaTVprogramWebService.asmx/getAreaString";

        public static string GetURL(string url)
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
