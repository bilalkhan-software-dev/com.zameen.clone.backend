using com.zameen.Data;
using com.zameen.Models;
using com.zameen.Models.Dto.Response;
using com.zameen.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace com.zameen.Repositories.Implementation;

public class SearchLogRepository(ApplicationDbContext context)
    : GenericRepository<SearchLog, int>(context),
        ISearchLogRepository
{
    public async Task<List<SearchLog>> GetLogsSinceAsync(DateTime cutoff)
    {
        return await _dbSet.AsNoTracking().Where(l => l.SearchedAt >= cutoff).ToListAsync();
    }

    public async Task<List<LocationCount>> GetTrendingLocationsRawAsync(DateTime cutoff, int top)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(l => l.SearchedAt >= cutoff && !string.IsNullOrWhiteSpace(l.Location))
            .GroupBy(l => l.Location)
            .Select(g => new LocationCount { Location = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(top)
            .ToListAsync();
    }

    public async Task<List<TrendingLocationData>> GetTrendingLocationsByCityAsync(
        string city,
        DateTime fromDate,
        DateTime toDate
    )
    {
        return await _dbSet
            .AsNoTracking()
            .Where(l => l.City == city && l.SearchedAt >= fromDate && l.SearchedAt <= toDate)
            .GroupBy(l => new { l.Location, l.SearchedAt.Date })
            .Select(g => new TrendingLocationData
            {
                Location = g.Key.Location,
                Date = g.Key.Date,
                SearchCount = g.Count(),
            })
            .OrderBy(d => d.Date)
            .ThenBy(d => d.Location)
            .ToListAsync();
    }
}
