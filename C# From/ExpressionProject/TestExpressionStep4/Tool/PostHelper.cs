using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestExpressionStep4
{
    public class PostHelper
    {
        internal static string BuildUrl(SearchDto dto, string url = null)
        {
            if (dto == null)
                throw new ArgumentNullException("criteria");

            StringBuilder sbUrl = new StringBuilder(url ?? "http://linqtocnblogs.cloudapp.net/");
            StringBuilder sbParameter = new StringBuilder();

            if (!string.IsNullOrEmpty(dto.Title))
                AppendParameter(sb: sbParameter, name: "Title", value: dto.Title);
            if (!string.IsNullOrEmpty(dto.Author))
                AppendParameter(sb: sbParameter, name: "Author", value: dto.Author);
            if (dto.Start.HasValue)
                AppendParameter(sb: sbParameter, name: "Start", value: dto.Start.Value.ToString());
            if (dto.End.HasValue)
                AppendParameter(sb: sbParameter, name: "End", value: dto.End.Value.ToString());
            if (dto.MinDiggs > 0)
                AppendParameter(sb: sbParameter, name: "MinDiggs", value: dto.MinDiggs.ToString());
            if (dto.MinViews > 0)
                AppendParameter(sb: sbParameter, name: "MinViews", value: dto.MinViews.ToString());
            if (dto.MinComments > 0)
                AppendParameter(sb: sbParameter, name: "MinComments", value: dto.MinComments.ToString());
            if (dto.MaxDiggs > 0)
                AppendParameter(sb: sbParameter, name: "MaxDiggs", value: dto.MaxDiggs.ToString());
            if (dto.MaxViews > 0)
                AppendParameter(sb: sbParameter, name: "MaxViews", value: dto.MaxViews.ToString());
            if (dto.MaxComments > 0)
                AppendParameter(sb: sbParameter, name: "MaxComments", value: dto.MaxComments.ToString());

            if (sbParameter.Length > 0)
                sbUrl.AppendFormat("?{0}", sbParameter.ToString());
            return sbUrl.ToString();
        }

        private static void AppendParameter(StringBuilder sb, string name, string value)
        {
            if (sb.Length > 0)
                sb.Append("&");
            sb.AppendFormat("{0}={1}", name, value);
        }

        internal static IEnumerable<Post> FindWebEntity(string url)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(url);
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
            using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                string body = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<Post>>(body);
            }
        }
    }
}
