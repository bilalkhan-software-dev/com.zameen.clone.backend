using com.zameen.Models;
using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;

namespace com.zameen.Repositories.Interfaces;

public interface IPriceTrendRepository : IGenericRepository<PriceTrend, int>
{
    Task<PriceTrendResponse?> GetPriceTrendAsync(
        string location,
        PropertyType propertyType,
        string sizeRange,
        string range
    );
}
