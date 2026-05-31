using System.Security.Claims;
using com.zameen.Models.Dto.Request;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnquiryController(IEnquiryService _enquiryService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] CreateEnquiryRequest request)
    {
        var result = await _enquiryService.SendEnquiryAsync(request);
        return Ok(result);
    }

    [HttpGet("property/{propertyId}")]
    [Authorize(Policy = "AdminAndAgentOnly")]
    public async Task<IActionResult> GetForProperty(
        int propertyId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10
    )
    {
        // In production, verify that the requesting agent owns the property.
        var result = await _enquiryService.GetEnquiriesForPropertyAsync(propertyId, page, size);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _enquiryService.GetEnquiryByIdAsync(id);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _enquiryService.DeleteEnquiryAsync(id, userId);
        return Ok(result);
    }
}
