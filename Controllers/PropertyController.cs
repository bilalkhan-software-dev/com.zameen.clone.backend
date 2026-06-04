using System.Security.Claims;
using com.zameen.Models.Dto.Request;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyController(
    IPropertyService _propertyService,
    ITrendingService _trendingService
) : ControllerBase
{
    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    // Public search
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] PropertyFilterParams filters)
    {
        var result = await _propertyService.SearchAsync(filters);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _propertyService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpGet("locations")]
    [AllowAnonymous]
    public async Task<ActionResult> GetLocationSuggestions(
        [FromQuery] string city,
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10
    )
    {
        var result = await _propertyService.GetLocationSuggestionsByCity(
            city,
            searchTerm,
            page,
            size
        );
        return Ok(result);
    }

    [HttpGet("trending/locations")]
    [AllowAnonymous]
    public async Task<ActionResult> GetTrendingLocations([FromQuery] int top = 10)
    {
        var result = await _trendingService.GetTrendingLocationsAsync(top);
        return Ok(result);
    }

    [HttpGet("trending")]
    [AllowAnonymous]
    public async Task<ActionResult> GetTrending([FromQuery] int count = 6)
    {
        var result = await _trendingService.GetTrendingPropertiesAsync(count);
        return Ok(result);
    }

    // Agent operations
    [HttpPost]
    [Authorize(Policy = "AgentOnly")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        var userId = GetUserId();
        var result = await _propertyService.CreateAsync(request, userId);
        return Ok(result);
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyRequest request)
    {
        var userId = GetUserId();
        var result = await _propertyService.UpdateAsync(id, request, userId);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var result = await _propertyService.DeleteAsync(id, userId);
        return Ok(result);
    }

    [HttpPut("{id}/toggle-active")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var userId = GetUserId();
        var result = await _propertyService.ToggleActiveAsync(id, userId);
        return Ok(result);
    }

    // Agent's own properties
    [HttpGet("my-properties")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<IActionResult> GetMyProperties(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10
    )
    {
        var userId = GetUserId();
        var result = await _propertyService.GetPropertiesByAgentAsync(userId, page, size);
        return Ok(result);
    }
}
