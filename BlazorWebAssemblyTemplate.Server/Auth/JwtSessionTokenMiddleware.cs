using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorWebAssemblyTemplate.Server.Auth
{
    public class JwtSessionTokenMiddleware(RequestDelegate next, IConfiguration config, IAuthService authService)
    {
        private readonly RequestDelegate _next = next;
        private readonly string _jwtSecret = config["Jwt:Key"];
        private readonly IAuthService _authService = authService;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Session.TryGetValue("JwtToken", out var tokenBytes))
            {
                var token = Encoding.UTF8.GetString(tokenBytes);
                var handler = new JwtSecurityTokenHandler();

                try
                {
                    var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role,
                        ValidateLifetime = true
                    }, out var validatedToken);

                    context.User = claimsPrincipal;

                    var jwtToken = validatedToken as JwtSecurityToken;
                    if (jwtToken != null && jwtToken.ValidTo < DateTime.UtcNow.AddMinutes(5))
                    {
                        var newToken = await _authService.RenewToken();
                        if (newToken != null)
                        {
                            context.Session.SetString("JwtToken", newToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Token validation failed: {ex.Message}");
                }
            }

            await _next(context);
        }
    }
}
