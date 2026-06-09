using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;

namespace com.zameen.Services.Interfaces;

public interface IPriceTrendService
{
    Task<ApiResponse<PriceTrendResponse>> GetPriceTrendForProperty(
        string city,
        string location,
        PropertyType propertyType,
        PropertyPurpose propertyPurpose,
        string sizeRange,
        string range
    );
}
