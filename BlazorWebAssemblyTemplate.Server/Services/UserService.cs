using BlazorWebAssemblyTemplate.Server.Repositories;
using BlazorWebAssemblyTemplate.Shared.Models;

namespace BlazorWebAssemblyTemplate.Server.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsers();
    }

    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<IEnumerable<User>> GetUsers() => await _userRepository.GetUsers();
    }
}
