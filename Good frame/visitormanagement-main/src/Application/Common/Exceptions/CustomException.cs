using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Blazor.Application.Common.Exceptions
{
    public class CustomException : Exception
    {
        /// <summary>
        /// 错误信息集合【自定义异常属性】
        /// </summary>
        public List<string>? ErrorMessages { get; }

        /// <summary>
        /// Http状态码【自定义异常属性】
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// 构造自定义异常。异常信息，错误信息集合，Http状态码
        /// </summary>
        /// <param name="message">异常信息</param>
        /// <param name="errors">错误信息集合</param>
        /// <param name="statusCode">Http状态码</param>
        public CustomException(string message, List<string>? errors = default, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            : base(message)
        {
            ErrorMessages = errors;
            StatusCode = statusCode;
        }
    }
}