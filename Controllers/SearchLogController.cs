using com.zameen.Models.Dto.Request;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchLogController(ITrendingService trendingService) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> LogSearch([FromBody] CreateSearchLogRequest request)
    {
        await trendingService.LogSearchAsync(request);
        return Ok();
    }

    [HttpGet("trending/locations/by-city")]
    [AllowAnonymous]
    public async Task<IActionResult> GetTrendingLocationsByCity(
        [FromQuery] string city,
        [FromQuery] int days = 30
    )
    {
        var result = await trendingService.GetTrendingLocationsByCityAsync(city, days);
        return Ok(result);
    }
}
