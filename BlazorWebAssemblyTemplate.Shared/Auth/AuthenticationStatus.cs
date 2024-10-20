namespace BlazorWebAssemblyTemplate.Shared.Auth
{
    public class AuthenticationStatus
    {
        public bool IsAuthenticated { get; set; }
        public string? Role { get; set; }
    }
}
