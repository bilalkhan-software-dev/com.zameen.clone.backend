using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;

namespace com.zameen.Services.Interfaces;

public interface IPriceTrendService
{
    Task<ApiResponse<PriceTrendResponse>> GetPriceTrendForProperty(
        string location,
        PropertyType propertyType,
        string sizeRange,
        string range
    );
}
