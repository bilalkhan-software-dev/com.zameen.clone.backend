namespace com.zameen.Models.Dto.Response;

public class PriceTrendResponse
{
    public decimal CurrentPrice { get; set; }
    public decimal PriceChange { get; set; }
    public decimal PercentChange { get; set; }
    public List<TrendPoint> History { get; set; } = new();
    public decimal? SixMonthsAgo { get; set; }
    public decimal? TwelveMonthsAgo { get; set; }
    public decimal? TwentyFourMonthsAgo { get; set; }
}
