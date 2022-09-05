using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Blazor.Application.Services.MessageService
{
    /// <summary>
    /// 往指定网址发送表单信息
    /// </summary>
    public class SMSMessageService
    {
        private const string host = "https://dfsns.market.alicloudapi.com";
        private const string path = "/data/send_sms";
        private const string method = "POST";
        private const string appcode = "e37a9c2841974dbbad7e517f1d36940e";
        private readonly ILogger<SMSMessageService> logger;

        public SMSMessageService(ILogger<SMSMessageService> logger)
        {
            this.logger = logger;
        }
        public async Task Send(string to, string[] args, string templ = "TPL_0000")
        {
            try
            {
                string url = host + path;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "APPCODE " + appcode);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    List<KeyValuePair<string, string>> nvc = new List<KeyValuePair<string, string>>();
                    nvc.Add(new KeyValuePair<string, string>("content", string.Join(',', args)));
                    nvc.Add(new KeyValuePair<string, string>("phone_number", to));
                    nvc.Add(new KeyValuePair<string, string>("template_id", templ));
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new FormUrlEncodedContent(nvc)
                    };
                    HttpResponseMessage res = await client.SendAsync(req);
                    string content = await res.Content.ReadAsStringAsync();
                    logger.LogInformation($"Send to {to}:{string.Join(',', args)}, result:{content}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Send to {to}:{string.Join(',', args)}");
            }

        }
    }
}
