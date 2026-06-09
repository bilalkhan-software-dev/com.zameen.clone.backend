using com.zameen.Models.Enums;

namespace com.zameen.Models;

public class PriceTrend : AbstractEntity
{
    public int Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public PropertyPurpose PropertyPurpose { get; set; }
    public string SizeRange { get; set; } = string.Empty;
    public DateTime RecordedDate { get; set; }
    public decimal AveragePrice { get; set; }
}
