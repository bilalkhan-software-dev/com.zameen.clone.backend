namespace com.zameen.Models.Dto.Response;

public class PropertyResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal AreaSize { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public List<string> PropertyPics { get; set; } = [];
    public string PropertyType { get; set; } = string.Empty;
    public string PropertyPurpose { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public AgentResponse Agent { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Dictionary<string, object> Amenities { get; set; } = [];
}
