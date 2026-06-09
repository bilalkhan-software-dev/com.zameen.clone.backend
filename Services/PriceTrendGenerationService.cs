using com.zameen.Data;
using com.zameen.Models;
using Microsoft.EntityFrameworkCore;

namespace com.zameen.Services;

public class PriceTrendGenerationService(
    IServiceScopeFactory _scopeFactory,
    ILogger<PriceTrendGenerationService> _logger,
    IConfiguration _configuration
) : BackgroundService
{
    private bool _historyGenerated = false;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // ---------- Optional historical back‑fill ----------
        var generateHistory = _configuration.GetValue<bool>("PriceTrend:GenerateHistory", true);
        if (generateHistory && !_historyGenerated)
        {
            await GenerateHistoricalTrends(stoppingToken);
            _historyGenerated = true;
        }
        else if (!generateHistory)
        {
            _logger.LogInformation("Historical trend generation skipped (GenerateHistory=false).");
            _historyGenerated = true; // mark as done so we don't try again
        }

        // 2. Then schedule monthly runs
        var targetDay = _configuration.GetValue<int>("PriceTrend:RunDay", 1);
        var targetHour = _configuration.GetValue<int>("PriceTrend:RunHour", 2);
        var targetMinute = _configuration.GetValue<int>("PriceTrend:RunMinute", 0);

        _logger.LogInformation(
            "Scheduled monthly run on day {Day} at {Hour:D2}:{Minute:D2} UTC",
            targetDay,
            targetHour,
            targetMinute
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextRun = new DateTime(now.Year, now.Month, targetDay, targetHour, targetMinute, 0);
            if (nextRun <= now)
                nextRun = nextRun.AddMonths(1);

            // Edge case: day not in month
            if (targetDay > DateTime.DaysInMonth(nextRun.Year, nextRun.Month))
                nextRun = new DateTime(
                    nextRun.Year,
                    nextRun.Month,
                    DateTime.DaysInMonth(nextRun.Year, nextRun.Month),
                    targetHour,
                    targetMinute,
                    0
                );

            var delay = nextRun - now;
            _logger.LogInformation("Next run at {NextRun} (in {Delay})", nextRun, delay);

            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, stoppingToken);

            await RunOnce(stoppingToken);
        }
    }

    /// <summary>
    /// Generates trends for all months from the earliest property to last month.
    /// </summary>
    private async Task GenerateHistoricalTrends(CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Starting historical trend back‑fill...");
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Find the earliest property creation date
            var firstDate = await dbContext
                .Properties.Where(p => p.IsActive)
                .MinAsync(p => p.CreatedAt, ct);

            // No properties? nothing to do.
            if (firstDate == default)
            {
                _logger.LogInformation(
                    "No active properties found. Skipping historical generation."
                );
                return;
            }

            // Start from the first month that is >= firstDate, go up to last month
            var startMonth = new DateTime(firstDate.Year, firstDate.Month, 1);
            var endMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(
                1
            );

            for (var month = startMonth; month <= endMonth; month = month.AddMonths(1))
            {
                await GenerateMonthlyTrendsForDate(dbContext, month);
            }

            _logger.LogInformation("Historical back‑fill completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during historical trend generation.");
        }
    }

    /// <summary>
    /// Generates trends for a specific month (targetDate = first day of that month).
    /// </summary>
    private async Task GenerateMonthlyTrendsForDate(
        ApplicationDbContext dbContext,
        DateTime targetDate
    )
    {
        var activeProperties = await dbContext
            .Properties.Where(p => p.IsActive && p.CreatedAt <= targetDate) // only properties that existed by that month
            .Select(p => new
            {
                p.City,
                p.Location,
                p.PropertyType,
                p.PropertyPurpose,
                p.AreaSize,
                p.Price,
            })
            .ToListAsync();

        var groups = activeProperties
            .GroupBy(p => new
            {
                p.City,
                p.Location,
                p.PropertyType,
                p.PropertyPurpose,
                SizeRange = MapAreaToSizeRange(p.AreaSize),
            })
            .Select(g => new
            {
                g.Key.City,
                g.Key.Location,
                g.Key.PropertyType,
                g.Key.PropertyPurpose,
                g.Key.SizeRange,
                AveragePrice = g.Average(p => p.Price),
            });

        foreach (var group in groups)
        {
            var exists = await dbContext.PriceTrends.AnyAsync(pt =>
                pt.City == group.City
                && pt.Location == group.Location
                && pt.PropertyType == group.PropertyType
                && pt.PropertyPurpose == group.PropertyPurpose
                && pt.SizeRange == group.SizeRange
                && pt.RecordedDate == targetDate
            );

            if (!exists)
            {
                dbContext.PriceTrends.Add(
                    new PriceTrend
                    {
                        City = group.City,
                        Location = group.Location,
                        PropertyType = group.PropertyType,
                        PropertyPurpose = group.PropertyPurpose,
                        SizeRange = group.SizeRange,
                        RecordedDate = targetDate,
                        AveragePrice = group.AveragePrice,
                    }
                );
            }
        }

        await dbContext.SaveChangesAsync();
    }

    // Called by the monthly schedule for the previous month
    private async Task GenerateMonthlyTrends()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var targetDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(-1);
        await GenerateMonthlyTrendsForDate(dbContext, targetDate);
    }

    private async Task RunOnce(CancellationToken ct)
    {
        try
        {
            _logger.LogInformation(
                "Monthly price trend generation started at {Time}",
                DateTime.UtcNow
            );
            await GenerateMonthlyTrends();
            _logger.LogInformation("Monthly generation completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating monthly trends.");
        }
    }

    private static string MapAreaToSizeRange(decimal areaSqFt)
    {
        if (areaSqFt <= 500)
            return "0-500 sqft";
        if (areaSqFt <= 1000)
            return "500-1000 sqft";
        if (areaSqFt <= 2000)
            return "1000-2000 sqft";
        if (areaSqFt <= 5000)
            return "2000-5000 sqft";
        return "5000+ sqft";
    }
}
