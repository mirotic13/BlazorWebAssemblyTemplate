using BlazorWebAssemblyTemplate.Shared.Models;
using Dapper;
using System.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BlazorWebAssemblyTemplate.Shared.Request;
using BlazorWebAssemblyTemplate.Shared.Auth;

namespace BlazorWebAssemblyTemplate.Server.Auth
{
    public interface IAuthService
    {
        Task<bool> Login(LoginRequest request);
        Task Register(RegisterRequest request);
        Task<bool> UserExists(string email);
        AuthenticationStatus GetUserAuthenticationStatus();
    }

    public class AuthService(IDbConnection localConnection, IConfiguration config, IHttpContextAccessor httpContextAccessor) : IAuthService
    {
        private readonly IDbConnection _localConnection = localConnection;
        private readonly string _jwtSecret = config["Jwt:Key"];
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly int _tokenExpireMinutes = config.GetValue<int>("Jwt:ExpireMinutes");

        public async Task<bool> Login(LoginRequest request)
        {
            var query = "SELECT id, name, email, password, created_at AS CreatedAt, role FROM Users WHERE Email = @Email";
            var user = await _localConnection.QuerySingleOrDefaultAsync<User>(query, new { request.Email });

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                Console.WriteLine("Wrong user credentials");
                return false;
            }

            var token = GenerateJwtToken(user);

            _httpContextAccessor.HttpContext.Session.SetString("JwtToken", token);

            return true;
        }

        public async Task Register(RegisterRequest request)
        {
            var query = "INSERT INTO Users (name, email, password, role, created_at) VALUES (@Name, @Email, @Password, @Role, @CreatedAt)";
            request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await _localConnection.ExecuteAsync(query,
                new { request.Name, request.Email, request.Password, Role = request.Role.ToString(), CreatedAt = DateTime.Now });
        }

        public async Task<bool> UserExists(string email)
        {
            var query = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            var count = await _localConnection.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }

        public AuthenticationStatus GetUserAuthenticationStatus()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationStatus
                {
                    IsAuthenticated = false,
                    Role = null
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = tokenHandler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

                return new AuthenticationStatus
                {
                    IsAuthenticated = true,
                    Role = roleClaim?.Value
                };
            }
            catch
            {
                return new AuthenticationStatus
                {
                    IsAuthenticated = false,
                    Role = null
                };
            }
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_tokenExpireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
