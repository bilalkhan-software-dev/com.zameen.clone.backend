using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;

namespace com.zameen.Services.Interfaces;

public interface IPropertyService
{
    Task<ApiResponse<PagedResult<PropertyResponse>>> SearchAsync(PropertyFilterParams filters);
    Task<ApiResponse<PropertyResponse>> GetByIdAsync(int id);
    Task<ApiResponse<PropertyResponse>> CreateAsync(
        CreatePropertyRequest request,
        string agentUserId
    );
    Task<ApiResponse<PropertyResponse>> UpdateAsync(
        int id,
        UpdatePropertyRequest request,
        string agentUserId
    );

    /// <summary>
    /// Soft Delete IsActive=false
    /// </summary>
    Task<ApiResponse> DeleteAsync(int id, string agentUserId);
    Task<ApiResponse<PagedResult<PropertyResponse>>> GetPropertiesByAgentAsync(
        string agentUserId,
        int page,
        int size
    );
    Task<ApiResponse> ToggleActiveAsync(int id, string agentUserId);
}
