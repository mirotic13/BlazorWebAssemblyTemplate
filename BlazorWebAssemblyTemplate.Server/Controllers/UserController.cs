using BlazorWebAssemblyTemplate.Server.Services;
using BlazorWebAssemblyTemplate.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWebAssemblyTemplate.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _userService.GetUsers();

            return Ok(response);
        }
    }
}
