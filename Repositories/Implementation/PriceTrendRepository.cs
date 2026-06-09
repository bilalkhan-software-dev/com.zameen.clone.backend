using com.zameen.Data;
using com.zameen.Models;
using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;
using com.zameen.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace com.zameen.Repositories.Implementation;

public class PriceTrendRepository(ApplicationDbContext context)
    : GenericRepository<PriceTrend, int>(context),
        IPriceTrendRepository
{
    public async Task<PriceTrendResponse?> GetPriceTrendAsync(
        string city,
        string location,
        PropertyType propertyType,
        PropertyPurpose propertyPurpose,
        string sizeRange,
        string range
    )
    {
        var cutoff = range switch
        {
            "6m" => DateTime.UtcNow.AddMonths(-6),
            "1y" => DateTime.UtcNow.AddYears(-1),
            _ => DateTime.UtcNow.AddYears(-10),
        };

        var data = await _dbSet
            .Where(p =>
                p.Location == location
                && p.City == city
                && p.PropertyType == propertyType
                && p.PropertyPurpose == propertyPurpose
                && p.SizeRange == sizeRange
                && p.RecordedDate >= cutoff
            )
            .OrderBy(p => p.RecordedDate)
            .Select(p => new TrendPoint { Date = p.RecordedDate, Price = p.AveragePrice })
            .ToListAsync();

        if (data.Count == 0)
            return null;

        var current = data.Last().Price;
        var oldest = data.First().Price;
        var change = current - oldest;
        var percentChange = change / oldest * 100;

        // Optional: compute 6/12/24 months ago prices
        var now = DateTime.UtcNow;
        var sixMonthsAgo = data.FirstOrDefault(p => p.Date >= now.AddMonths(-6))?.Price;
        var twelveMonthsAgo = data.FirstOrDefault(p => p.Date >= now.AddYears(-1))?.Price;
        var twentyFourMonthsAgo = data.FirstOrDefault(p => p.Date >= now.AddYears(-2))?.Price;

        return new PriceTrendResponse
        {
            CurrentPrice = current,
            PriceChange = change,
            PercentChange = percentChange,
            History = data,
            SixMonthsAgo = sixMonthsAgo,
            TwelveMonthsAgo = twelveMonthsAgo,
            TwentyFourMonthsAgo = twentyFourMonthsAgo,
        };
    }
}
