using System.Collections.Generic;

namespace CleanArchitecture.Blazor.Application.Common.Exceptions
{
    /// <summary>
    /// 服务器内部异常
    /// </summary>
    public class InternalServerException : CustomException
    {
        public InternalServerException(string message, List<string>? errors = default)
            : base(message, errors, System.Net.HttpStatusCode.InternalServerError)
        {
        }
    }
}