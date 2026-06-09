using System.ComponentModel.DataAnnotations.Schema;
using com.zameen.Models.Enums;

namespace com.zameen.Models;

public class Property : AbstractEntity
{
    public int Id { get; set; }

    [ForeignKey("Agent")]
    public string AgentId { get; set; } = string.Empty;
    public required Agent Agent { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> PropertyPics { get; set; } = [];
    public decimal Price { get; set; }
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }

    // Using Square Feet As a Base Area Size Front end change it
    public decimal AreaSize { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsActive { get; set; } = true; // Soft Delete
    public PropertyStatus Status { get; set; } = PropertyStatus.PENDING;
    public PropertyType PropertyType { get; set; } = PropertyType.HOUSE;
    public PropertyPurpose PropertyPurpose { get; set; } = PropertyPurpose.BUY;
    public string AmenitiesJson { get; set; } = "{}";
}
