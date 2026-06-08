using com.zameen.Models.Enums;
using com.zameen.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace com.zameen.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PriceTrendController(IPriceTrendService priceTrendService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetTrends(
        [FromQuery] string city,
        [FromQuery] string location,
        [FromQuery] PropertyType propertyType,
        [FromQuery] PropertyPurpose propertyPurpose,
        [FromQuery] string sizeRange,
        [FromQuery] string range = "1y"
    ) // "6m", "1y", "max"
    {
        var result = await priceTrendService.GetPriceTrendForProperty(
            city,
            location,
            propertyType,
            propertyPurpose,
            sizeRange,
            range
        );

        return Ok(result);
    }
}
