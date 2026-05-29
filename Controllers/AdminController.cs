using com.zameen.Models.Dto.Request;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class UserManagementController(
        IUserService userService,
        ILogger<UserManagementController> logger
    ) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly ILogger<UserManagementController> _logger = logger;

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(result);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var result = await _userService.GetUserByIdAsync(userId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("users/roles")]
        public async Task<IActionResult> UpdateUserRoles(UpdateUserRolesDto dto)
        {
            var result = await _userService.UpdateUserRolesAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("users/status")]
        public async Task<IActionResult> ChangeUserStatus(ChangeUserStatusDto dto)
        {
            var result = await _userService.ChangeUserAccountStatusAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
