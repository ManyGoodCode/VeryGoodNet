using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CacheCow.Common.Helpers
{
	public static class HttpResponseMessageExtensions
	{
        /// <summary>
        /// 将HttpResponseMessage实例用TaskCompletionSource封装直接返回
        /// </summary>
		public static Task<HttpResponseMessage> ToTask(this HttpResponseMessage responseMessage)
		{
            TaskCompletionSource<HttpResponseMessage> taskCompletionSource = new TaskCompletionSource<HttpResponseMessage>();
			taskCompletionSource.SetResult(responseMessage);
			return taskCompletionSource.Task;
		}

        /// <summary>
        /// 获取HttpResponseMessage的过期时间
        /// </summary>
		public static DateTimeOffset? GetExpiry(this HttpResponseMessage response)
		{      
			if (response.Headers.CacheControl != null && response.Headers.CacheControl.MaxAge.HasValue)
			{
				return DateTimeOffset.UtcNow.Add(response.Headers.CacheControl.MaxAge.Value);
			}
           
			return response.Content != null && response.Content.Headers.Expires.HasValue
				? response.Content.Headers.Expires.Value
				: (DateTimeOffset?) null;
		}
        
        public static async Task WhatEnsureSuccessShouldHaveBeen(this HttpResponseMessage response)
        {
            if(!response.IsSuccessStatusCode)
            {
                if(response.Content == null)
                {
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new HttpRequestException($"Status: {response.StatusCode}\r\n Error: {error}");
                }
            }
        }
	}
}
