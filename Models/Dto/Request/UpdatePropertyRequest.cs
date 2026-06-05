using com.zameen.Models.Enums;

namespace com.zameen.Models.Dto.Request;

public class UpdatePropertyRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Price { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? ContactNumber { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public decimal? AreaSize { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public PropertyType? PropertyType { get; set; }
    public PropertyPurpose? PropertyPurpose { get; set; }

    public List<string>? PropertyPics { get; set; }
    public Dictionary<string, object>? Amenities { get; set; }
}
