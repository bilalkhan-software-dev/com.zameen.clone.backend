using com.zameen.Models.Enums;

namespace com.zameen.Models;

public class SearchLog
{
    public int Id { get; set; }
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public PropertyPurpose PropertyPurpose { get; set; }
    public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
}
