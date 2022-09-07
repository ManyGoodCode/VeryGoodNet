namespace CleanArchitecture.Blazor.Application.Common.Exceptions
{
    /// <summary>
    /// 禁止异常
    /// </summary>
    public class ForbiddenAccessException : CustomException
    {
        public ForbiddenAccessException(string message)
            : base(message, null, System.Net.HttpStatusCode.Forbidden) { }
    }
}
