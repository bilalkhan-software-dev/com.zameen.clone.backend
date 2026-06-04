using AutoMapper;
using com.zameen.Models;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;
using com.zameen.Repositories.Interfaces;
using com.zameen.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace com.zameen.Services.Implementation;

public class TrendingService(
    ISearchLogRepository _searchLogRepo,
    IPropertyRepository _propertyRepo,
    IMapper _mapper,
    IMemoryCache _cache
) : ITrendingService
{
    public async Task<ApiResponse<List<PropertyResponse>>> GetTrendingPropertiesAsync(int count = 6)
    {
        // Try cache
        if (
            _cache.TryGetValue("trending_properties", out List<PropertyResponse>? cached)
            && cached != null
        )
            return ApiResponse<List<PropertyResponse>>.Ok(cached);

        // Get trending locations from last 7 days
        var cutoff = DateTime.UtcNow.AddDays(-7);
        var trendingLocs = await _searchLogRepo.GetTrendingLocationsRawAsync(cutoff, top: 10);
        var locations = trendingLocs.Select(l => l.Location).ToList();

        if (!locations.Any())
            return ApiResponse<List<PropertyResponse>>.Ok(new List<PropertyResponse>());

        // Fetch properties in those locations, approved and active
        var properties = await _propertyRepo
            .GetQueryable()
            .Where(p =>
                p.IsActive && p.Status == PropertyStatus.APPROVED && locations.Contains(p.Location)
            )
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .Include(p => p.Agent)
            .AsNoTracking()
            .ToListAsync();

        var response = _mapper.Map<List<PropertyResponse>>(properties);

        // Cache for 2 hours
        _cache.Set("trending_properties", response, TimeSpan.FromHours(2));

        return ApiResponse<List<PropertyResponse>>.Ok(response);
    }

    public async Task<ApiResponse> LogSearchAsync(CreateSearchLogRequest request)
    {
        var log = new SearchLog
        {
            Location = request.Location,
            City = request.City,
            PropertyType = request.PropertyType,
            PropertyPurpose = request.PropertyPurpose,
            SearchedAt = DateTime.UtcNow,
        };

        await _searchLogRepo.AddAsync(log);

        // Invalidate cache after new log
        _cache.Remove("trending_locations");
        _cache.Remove("trending_properties");

        return ApiResponse.Ok("Search logged successfully");
    }

    public async Task<ApiResponse<List<TrendingLocationDto>>> GetTrendingLocationsAsync(
        int top = 10
    )
    {
        // Try cache
        if (
            _cache.TryGetValue("trending_locations", out List<TrendingLocationDto>? cached)
            && cached != null
        )
            return ApiResponse<List<TrendingLocationDto>>.Ok(cached);

        var cutoff = DateTime.UtcNow.AddDays(-7);
        var rawLocations = await _searchLogRepo.GetTrendingLocationsRawAsync(cutoff, top);

        var result = rawLocations
            .Select(l => new TrendingLocationDto { Location = l.Location, SearchCount = l.Count })
            .ToList();

        // Cache for 2 hours
        _cache.Set("trending_locations", result, TimeSpan.FromHours(2));

        return ApiResponse<List<TrendingLocationDto>>.Ok(result);
    }
}
