using com.zameen.Models;
using com.zameen.Models.Dto.Response;

namespace com.zameen.Repositories.Interfaces;

public interface ISearchLogRepository : IGenericRepository<SearchLog, int>
{
    Task<List<SearchLog>> GetLogsSinceAsync(DateTime cutoff);
    Task<List<LocationCount>> GetTrendingLocationsRawAsync(DateTime cutoff, int top);
    Task<List<TrendingLocationData>> GetTrendingLocationsByCityAsync(
        string city,
        DateTime fromDate,
        DateTime toDate
    );
}
