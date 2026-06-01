using com.zameen.Models.Dto.Request;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController(IUserService _userService) : ControllerBase
    {
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10
        )
        {
            var result = await _userService.GetAllUsersAsync(page, size);
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
