using com.zameen.Models;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;

namespace com.zameen.Repositories.Interfaces;

public interface IPropertyRepository : IGenericRepository<Property, int>
{
    Task<PagedResult<Property>> SearchAsync(PropertyFilterParams filters);
    Task<PagedResult<Property>> GetPropertiesByAgentAsync(string agentId, int page, int size);
    Task<PagedResult<string>> GetLocationSuggestionsByCityAsync(
        string city,
        string searchTerm,
        int page,
        int size
    );
    Task<Property?> GetPropertyDetailById(int propertyId);
    Task<bool> ExistsByTitleAsync(string title, int? excludeId = null);
}
