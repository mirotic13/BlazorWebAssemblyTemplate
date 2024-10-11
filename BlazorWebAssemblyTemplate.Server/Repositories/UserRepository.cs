using BlazorWebAssemblyTemplate.Shared.Models;
using Dapper;
using System.Data;

namespace BlazorWebAssemblyTemplate.Server.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsers();
    }

    public class UserRepository(IDbConnection localDbConnection) : IUserRepository
    {
        private readonly IDbConnection _localDbConnection = localDbConnection;

        public async Task<IEnumerable<User>> GetUsers()
        {
            var query = @"SELECT id, name, email, password, created_at AS CreatedAt FROM users";
            return await _localDbConnection.QueryAsync<User>(query);
        }
    }
}
