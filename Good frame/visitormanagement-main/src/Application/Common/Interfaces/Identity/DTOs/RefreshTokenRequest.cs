namespace CleanArchitecture.Blazor.Application.Common.Interfaces.Identity.DTOs
{

    public class RefreshTokenRequest
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
