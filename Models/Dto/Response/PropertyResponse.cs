namespace com.zameen.Models.Dto.Response;

public class PropertyResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal AreaSize { get; set; }
    public string AreaUnit { get; set; } = string.Empty;
    public List<string> PropertyPics { get; set; } = [];
    public string PropertyType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public string AgentId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
