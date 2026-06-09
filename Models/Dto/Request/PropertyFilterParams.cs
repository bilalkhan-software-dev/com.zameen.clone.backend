using com.zameen.Models.Enums;

namespace com.zameen.Models.Dto.Request;

public class PropertyFilterParams
{
    public string? City { get; set; }
    public string? Location { get; set; }
    public PropertyType? PropertyType { get; set; }
    public PropertyStatus? Status { get; set; }

    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinBedrooms { get; set; }
    public int? MaxBedrooms { get; set; }

    public int? MinBathrooms { get; set; }
    public int? MaxBathrooms { get; set; }
    public decimal? MinAreaSize { get; set; }
    public decimal? MaxAreaSize { get; set; }
    public string? SearchTerm { get; set; } // title or description

    // Pagination & Sorting
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } // e.g., "Price", "CreatedAt", "AreaSize"
    public bool IsDescending { get; set; } = false;
}
