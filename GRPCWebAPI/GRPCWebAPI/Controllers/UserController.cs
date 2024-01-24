//UserController.cs
using GRPCWebAPI.DTOs;
using GRPCWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GRPCWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        IUserService _userService;
        public UserController( IUserService userService)
        {
            _userService= userService;
        }
        [
            Route($"{nameof(Login)}"),
            HttpPost
        ]
        [AllowAnonymous]
        public async Task<IActionResult> Login (UserLoginRequest request)
        {
            var result = await _userService.Login(request);
           if (result.Success)
            {
                return Ok(result);
               
            }
            else
            {
                return NotFound(result);
            }
        }

        [
            Route($"{nameof(Registration)}"),
            HttpPost
        ]
        [AllowAnonymous]
        public async Task<IActionResult> Registration(UserRegistrationRequest request)
        {
            var result = await _userService.Registration(request);
            if (result.Success)
            {
                return Ok(result);

            }
            else
            {
                return StatusCode(402);
            }
        }
        [
            Route($"{nameof(GetUsers)}"),
            HttpGet
        ]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetUsers());
        }
    }
}
