using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;

namespace com.zameen.Services.Interfaces;

public interface IEnquiryService
{
    Task<ApiResponse<EnquiryResponse>> SendEnquiryAsync(CreateEnquiryRequest request);
    Task<ApiResponse<EnquiryResponse>> GetEnquiryByIdAsync(int id);
    Task<ApiResponse<PagedResult<EnquiryResponse>>> GetEnquiriesForPropertyAsync(
        int propertyId,
        int page,
        int size
    );
    Task<ApiResponse<PagedResult<EnquiryResponse>>> GetEnquiriesByAgentAsync(
        string agent,
        int page,
        int size
    );
    Task<ApiResponse> DeleteEnquiryAsync(int id, string adminUserId);

    Task<ApiResponse<PagedResult<EnquiryResponse>>> GetAllEnquiries(
        int page,
        int size,
        string sortBy,
        bool isNewest
    );
}
