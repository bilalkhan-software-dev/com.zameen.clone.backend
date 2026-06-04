using com.zameen.Models.Enums;

namespace com.zameen.Models.Dto.Request;

public class CreatePropertyRequest
{
    public string Title { get; set; } = string.Empty; // keep non-null with default
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal AreaSize { get; set; }
    public PropertyType PropertyType { get; set; }
    public PropertyPurpose PropertyPurpose { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public List<string>? PropertyPics { get; set; }
    public Dictionary<string, object>? Amenities { get; set; }
}
