namespace CleanArchitecture.Blazor.Application.Common.Exceptions
{
    /// <summary>
    /// 未找到资源异常
    /// </summary>
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message)
            : base(message, null, System.Net.HttpStatusCode.NotFound)
        {
        }

        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.", null, System.Net.HttpStatusCode.NotFound)
        {
        }
    }
}
