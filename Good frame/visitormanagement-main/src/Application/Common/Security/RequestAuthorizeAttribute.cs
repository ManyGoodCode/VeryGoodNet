namespace CleanArchitecture.Blazor.Application.Common.Security
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequestAuthorizeAttribute : Attribute
    {
        public RequestAuthorizeAttribute() { }

        public string Roles { get; set; }

        public string Policy { get; set; }
    }
}