using com.zameen.Models.Enums;

namespace com.zameen.Models.Dto.Request;

public class CreatePropertyRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Price { get; set; } = 0;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<string> PropertyPics { get; set; } = [];
    public int Bedrooms { get; set; } = 0;
    public int Bathrooms { get; set; } = 0;
    public decimal AreaSize { get; set; }
    public AreaUnit AreaUnit { get; set; }
    public PropertyType PropertyType { get; set; }
}
