using System.Security.Claims;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Enums;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController(IAgentService _agentService) : ControllerBase
{
    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // ─── Agent's own profile ──────────
    [HttpGet("me")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<IActionResult> GetMyProfile()
    {
        var result = await _agentService.GetAgentByUserIdAsync(GetUserId());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAgentAccount([FromBody] RegisterRequest request)
    {
        var result = await _agentService.CreateAgentAsync(request, GetUserId());
        return Ok(result);
    }

    [HttpPut("me")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateAgentRequest request)
    {
        var result = await _agentService.UpdateAgentAsync(GetUserId(), request);
        return Ok(result);
    }

    [HttpPut("{agentId}/status")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ChangeAgentStatus(
        string agentId,
        [FromBody] AccountStatus newStatus
    )
    {
        var result = await _agentService.ChangeAgentStatusAsync(agentId, newStatus);
        return Ok(result);
    }

    // ─── Admin operations ─────────────
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAgents(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] AccountStatus? status = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool descending = true
    )
    {
        var result = await _agentService.GetAgentsAsync(page, size, status, sortBy, descending);
        return Ok(result);
    }

    [HttpGet("{agentId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAgentById(string agentId)
    {
        var result = await _agentService.GetAgentByIdAsync(agentId);
        return Ok(result);
    }

    [HttpPut("{agentId}/approve")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ApproveAgent(string agentId)
    {
        var result = await _agentService.ApproveAgentAsync(agentId);
        return Ok(result);
    }

    [HttpPut("{agentId}/reject")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RejectAgent(string agentId)
    {
        var result = await _agentService.RejectAgentAsync(agentId);
        return Ok(result);
    }

    [HttpDelete("{agentId}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteAgent(string agentId)
    {
        var adminUserId = GetUserId();
        var result = await _agentService.DeleteAgentAsync(agentId, adminUserId);
        return Ok(result);
    }
}
