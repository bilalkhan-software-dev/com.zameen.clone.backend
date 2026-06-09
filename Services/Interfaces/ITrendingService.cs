using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;

namespace com.zameen.Services.Interfaces;

public interface ITrendingService
{
    Task<ApiResponse> LogSearchAsync(CreateSearchLogRequest createSearchLogRequest);
    Task<ApiResponse<List<PropertyResponse>>> GetTrendingPropertiesAsync(int count = 6);
    Task<ApiResponse<List<TrendingLocationDto>>> GetTrendingLocationsAsync(int top = 10);
    Task<ApiResponse<List<TrendingLocationData>>> GetTrendingLocationsByCityAsync(
        string city,
        int days = 30
    );
}
