using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;
using com.zameen.Repositories.Interfaces;
using com.zameen.Services.Interfaces;

namespace com.zameen.Services.Implementation;

public class PriceTrendService(IPriceTrendRepository priceTrendRepo) : IPriceTrendService
{
    public async Task<ApiResponse<PriceTrendResponse>> GetPriceTrendForProperty(
        string city,
        string location,
        PropertyType propertyType,
        PropertyPurpose propertyPurpose,
        string sizeRange,
        string range
    )
    {
        var result = await priceTrendRepo.GetPriceTrendAsync(
            city,
            location,
            propertyType,
            propertyPurpose,
            sizeRange,
            range
        );
        return ApiResponse<PriceTrendResponse>.Ok(result!);
    }
}
