using System.Security.Claims;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Enums;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController(
        IUserService userService,
        IPropertyService propertyService,
        IAgentService agentService,
        IEnquiryService enquiryService
    ) : ControllerBase
    {
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] AccountStatus? accountStatus = null
        )
        {
            var result = await userService.GetAllUsersAsync(page, size, accountStatus);
            return Ok(result);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var result = await userService.GetUserByIdAsync(userId);
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("users/roles")]
        public async Task<IActionResult> UpdateUserRoles(UpdateUserRolesDto dto)
        {
            var result = await userService.UpdateUserRolesAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("users/status")]
        public async Task<IActionResult> ChangeUserStatus(ChangeUserStatusDto dto)
        {
            var result = await userService.ChangeUserAccountStatusAsync(dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await userService.DeleteUserAsync(userId);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // ─── Admin Property Management ──────────────────────
        [HttpGet("properties")]
        public async Task<IActionResult> GetAllProperties(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] bool isNewest = true
        )
        {
            var result = await propertyService.GetAllProperties(page, size, sortBy, isNewest);
            return Ok(result);
        }

        [HttpPatch("properties/{id}/status")]
        public async Task<IActionResult> ChangePropertyStatus(
            int id,
            [FromBody] PropertyStatus newStatus
        )
        {
            var result = await propertyService.UpdatePropertyStatus(id, newStatus);
            return Ok(result);
        }

        // ─── Admin Agent Management ─────────────────────────
        [HttpPut("agents/{agentId}/status")]
        public async Task<IActionResult> ChangeAgentStatus(
            string agentId,
            [FromBody] AccountStatus newStatus
        )
        {
            var result = await agentService.ChangeAgentStatusAsync(agentId, newStatus);
            return Ok(result);
        }

        [HttpPut("agents/{agentId}/approve")]
        public async Task<IActionResult> ApproveAgent(string agentId)
        {
            var result = await agentService.ApproveAgentAsync(agentId);
            return Ok(result);
        }

        [HttpPut("agents/{agentId}/reject")]
        public async Task<IActionResult> RejectAgent(string agentId)
        {
            var result = await agentService.RejectAgentAsync(agentId);
            return Ok(result);
        }

        [HttpDelete("agents/{agentId}")]
        public async Task<IActionResult> DeleteAgent(string agentId)
        {
            var adminUserId = GetUserId();
            var result = await agentService.DeleteAgentAsync(agentId, adminUserId);
            return Ok(result);
        }

        [HttpGet("enquiries")]
        public async Task<IActionResult> GetAllEnquiries(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] bool isNewest = true
        )
        {
            var result = await enquiryService.GetAllEnquiries(page, size, sortBy, isNewest);
            return Ok(result);
        }

        [HttpDelete("enquiries/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await enquiryService.DeleteEnquiryAsync(id, userId);
            return Ok(result);
        }
    }
}
