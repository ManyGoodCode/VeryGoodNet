using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CacheCow.Client
{
    /// <summary>
    /// HttpResponseMessage ， HttpResponseMessage 与 Stream的序列化 ，反序列化操作异步接口
    /// </summary>
	public interface IHttpMessageSerializerAsync
	{
        /// <summary>
        /// 将 HttpResponseMessage 信息数据序列化到 Stream 中
        /// </summary>
		Task SerializeAsync(HttpResponseMessage response, Stream stream);

        /// <summary>
        /// 将 HttpRequestMessage 信息数据序列化到 Stream 中
        /// </summary>
		Task SerializeAsync(HttpRequestMessage request, Stream stream);

        /// <summary>
        /// 将 Stream 信息数据反序列化到 HttpResponseMessage 中
        /// </summary>
		Task<HttpResponseMessage> DeserializeToResponseAsync(Stream stream);

        /// <summary>
        /// 将 Stream 信息数据反序列化到 HttpRequestMessage 中
        /// </summary>
        Task<HttpRequestMessage> DeserializeToRequestAsync(Stream stream);
	}
}
