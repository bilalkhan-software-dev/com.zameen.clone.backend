using System.Security.Claims;
using com.zameen.Models.Dto.Request;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _userService.GetProfileAsync(GetUserId());
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _userService.ChangePasswordAsync(GetUserId(), request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpGet("debug")]
    public IActionResult Debug()
    {
        return Ok(
            new
            {
                IsAuth = User.Identity?.IsAuthenticated,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }),
            }
        );
    }
}
