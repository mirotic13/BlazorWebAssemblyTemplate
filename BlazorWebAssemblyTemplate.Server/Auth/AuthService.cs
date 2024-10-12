using BlazorWebAssemblyTemplate.Shared.Models;
using Dapper;
using System.Data;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BlazorWebAssemblyTemplate.Shared.Request;

namespace BlazorWebAssemblyTemplate.Server.Auth
{
    public interface IAuthService
    {
        Task<string?> Login(LoginRequest request);
        Task Register(RegisterRequest request);
        Task<bool> UserExists(string email);
    }

    public class AuthService(IDbConnection localConnection, IConfiguration config) : IAuthService
    {
        private readonly IDbConnection _localConnection = localConnection;
        private readonly string _jwtSecret = config["Jwt:Key"];

        public async Task<string?> Login(LoginRequest request)
        {
            var query = "SELECT id, name, email, password, created_at AS CreatedAt, role FROM Users WHERE Email = @Email";
            var user = await _localConnection.QuerySingleOrDefaultAsync<User>(query, new { request.Email });

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                Console.WriteLine("Wrong user credentials");
                return null;
            }

            return GenerateJwtToken(user);
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
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
