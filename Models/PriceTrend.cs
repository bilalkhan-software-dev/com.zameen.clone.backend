using com.zameen.Models.Enums;

namespace com.zameen.Models;

public class PriceTrend : AbstractEntity
{
    public int Id { get; set; }
    public string Location { get; set; } = string.Empty; // e.g., "DHA Phase 7 Block U"
    public PropertyType PropertyType { get; set; } // "HOUSE", "FLAT", "PLOT", etc.
    public PropertyPurpose PropertyPurpose { get; set; } // "RENT", "BUY"
    public string SizeRange { get; set; } = string.Empty; // "1-kanal", "5 marla", "1000-2000 sqft"
    public DateTime RecordedDate { get; set; } // month/year of the price
    public decimal AveragePrice { get; set; } // average price in PKR
    // public decimal AveragePricePerSqFt { get; set; }
}
