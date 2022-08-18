using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace CacheCow.Client
{
    /// <summary>
    /// HttpResponseMessage ， HttpResponseMessage 与 Stream的序列化 ，反序列化操作同步接口
    /// </summary>
    public interface IHttpMessageSerializer
	{
        /// <summary>
        /// 将 HttpResponseMessage 信息数据序列化到 Stream 中
        /// </summary>
        void Serialize(HttpResponseMessage response, Stream stream);

        /// <summary>
        /// 将 HttpRequestMessage 信息数据序列化到 Stream 中
        /// </summary>
        void Serialize(HttpRequestMessage request, Stream stream);

        /// <summary>
        /// 将 Stream 信息数据反序列化到 HttpResponseMessage 中
        /// </summary>
        HttpResponseMessage DeserializeToResponse(Stream stream);

        /// <summary>
        /// 将 Stream 信息数据反序列化到 HttpRequestMessage 中
        /// </summary>
        HttpRequestMessage DeserializeToRequest(Stream stream);
	}
}
