using BlazorWebAssemblyTemplate.Shared.Auth;
using System.Net.Http.Json;

namespace BlazorWebAssemblyTemplate.Client.Auth
{
    public interface IAuthService
    {
        Task<AuthenticationStatus> GetAuthenticationStatus();
    }

    public class AuthService(HttpClient httpClient) : IAuthService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<AuthenticationStatus> GetAuthenticationStatus()
        {
            var response = await _httpClient.GetAsync("api/auth/status");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthenticationStatus>();
            }

            return new AuthenticationStatus { IsAuthenticated = false };
        }
    }
}
