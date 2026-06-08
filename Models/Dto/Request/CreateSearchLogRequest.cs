using com.zameen.Models.Enums;

namespace com.zameen.Models.Dto.Request;

public class CreateSearchLogRequest
{
    public string Location { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public PropertyPurpose PropertyPurpose { get; set; }
}
