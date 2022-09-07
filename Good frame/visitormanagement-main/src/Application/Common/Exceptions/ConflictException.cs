namespace CleanArchitecture.Blazor.Application.Common.Exceptions
{
    /// <summary>
    /// 冲突异常继承自自定义异常
    /// </summary>
    public class ConflictException : CustomException
    {
        public ConflictException(string message)
            : base(message, null, System.Net.HttpStatusCode.Conflict)
        {
        }
    }
}
