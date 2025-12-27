using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.RequestModels.UserRequests;

namespace Zamm.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInput request)
        {
            return Ok(await _authService.RegisterAsync(request));
        }
        [HttpPost("confirm-register")]
        public async Task<IActionResult> ConfirmRegisterAccount(string confirmCode)
        {
            return Ok(await _authService.ConfirmRegisterAccount(confirmCode));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginInput request)
        {
            return Ok(await _authService.LoginAsync(request));
        }


        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserInfo()
        {
            var result = await _authService.GetUserInfoAsync();
            return Ok(result);
        }
        
        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return NoContent();
        }


        [HttpPut("update-profile/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserInput request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.UpdateUserAsync(userId, request);
            return Ok(result);
        }

        [HttpPut("change-password/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid userId, [FromBody] ChangePasswordInput request)
        {
            var result = await _authService.ChangePasswordAsync(userId, request);
            return Ok(result);
        }

        [HttpPost("{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddRolesToUser([FromRoute] Guid userId, [FromBody] List<string> roles)
        {
            if (roles == null || !roles.Any())
            {
                return BadRequest("Roles must be provided.");
            }

            var result = await _authService.AddRolesToUser(userId, roles);
            if (result == "Add roles successfully.")
            {
                return Ok(new { message = result });
            }
            return BadRequest(new { error = result });
        }
    }
}
