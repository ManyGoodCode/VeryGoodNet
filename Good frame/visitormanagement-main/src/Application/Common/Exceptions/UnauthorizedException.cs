namespace CleanArchitecture.Blazor.Application.Common.Exceptions
{
    /// <summary>
    /// 未授权异常
    /// </summary>
    public class UnauthorizedException : CustomException
    {
        public UnauthorizedException(string message)
           : base(message, null, System.Net.HttpStatusCode.Unauthorized)
        {
        }
    }
}

