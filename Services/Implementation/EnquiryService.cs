using AutoMapper;
using com.zameen.Exceptions;
using com.zameen.Models;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using com.zameen.Repositories.Interfaces;
using com.zameen.Services.Interfaces;

namespace com.zameen.Services.Implementation;

public class EnquiryService(
    IEnquiryRepository enquiryRepo,
    IPropertyRepository propertyRepo,
    IMapper mapper,
    ILogger<EnquiryService> logger
) : IEnquiryService
{
    public async Task<ApiResponse<EnquiryResponse>> SendEnquiryAsync(CreateEnquiryRequest request)
    {
        logger.LogInformation(
            "Enquiry from {Email} for property {PropertyId}",
            request.SenderEmail,
            request.PropertyId
        );

        var property = await propertyRepo.GetPropertyDetailById(request.PropertyId);
        if (property == null || !property.IsActive)
            throw new ResourceNotFoundException("Property not available.");

        var enquiry = mapper.Map<Enquiry>(request);
        enquiry.AgentId = property.Agent.Id;
        await enquiryRepo.AddAsync(enquiry);
        logger.LogInformation("Enquiry created ID {EnquiryId}", enquiry.Id);

        return ApiResponse<EnquiryResponse>.Ok(
            mapper.Map<EnquiryResponse>(enquiry),
            "Enquiry submitted."
        );
    }

    public async Task<ApiResponse<EnquiryResponse>> GetEnquiryByIdAsync(int id)
    {
        var enquiry = await enquiryRepo.GetByIdAsync(id);
        if (enquiry == null)
            throw new ResourceNotFoundException("Enquiry not found.");

        return ApiResponse<EnquiryResponse>.Ok(mapper.Map<EnquiryResponse>(enquiry));
    }

    public async Task<ApiResponse<PagedResult<EnquiryResponse>>> GetEnquiriesForPropertyAsync(
        int propertyId,
        int page,
        int size
    )
    {
        var paged = await enquiryRepo.GetByPropertyIdAsync(propertyId, page, size);
        var dtos = mapper.Map<IEnumerable<EnquiryResponse>>(paged.Items);

        return ApiResponse<PagedResult<EnquiryResponse>>.Ok(
            new PagedResult<EnquiryResponse>
            {
                Items = dtos,
                TotalCount = paged.TotalCount,
                Page = page,
                PageSize = size,
            }
        );
    }

    public async Task<ApiResponse<PagedResult<EnquiryResponse>>> GetEnquiriesByAgentAsync(
        string agentId,
        int page,
        int size
    )
    {
        var paged = await enquiryRepo.GetAllEnquiryByAgentAsync(agentId, page, size);
        var dtos = mapper.Map<IEnumerable<EnquiryResponse>>(paged.Items);

        return ApiResponse<PagedResult<EnquiryResponse>>.Ok(
            new PagedResult<EnquiryResponse>
            {
                Items = dtos,
                TotalCount = paged.TotalCount,
                Page = page,
                PageSize = size,
            }
        );
    }

    public async Task<ApiResponse> DeleteEnquiryAsync(int id, string adminUserId)
    {
        logger.LogWarning("Deleting enquiry {EnquiryId} by admin {AdminUserId}", id, adminUserId);

        var enquiry = await enquiryRepo.GetByIdAsync(id);
        if (enquiry == null)
            throw new ResourceNotFoundException("Enquiry not found.");

        enquiryRepo.Delete(enquiry);
        await enquiryRepo.SaveChangesAsync();
        logger.LogInformation("Enquiry {EnquiryId} deleted", id);
        return ApiResponse.Ok("Enquiry deleted.");
    }

    public async Task<ApiResponse<PagedResult<EnquiryResponse>>> GetAllEnquiries(
        int page,
        int size,
        string sortBy,
        bool isNewest
    )
    {
        var paged = await enquiryRepo.GetPagedAsync(page, size, sortBy, isNewest);
        var dtos = mapper.Map<IEnumerable<EnquiryResponse>>(paged.Items);

        return ApiResponse<PagedResult<EnquiryResponse>>.Ok(
            new PagedResult<EnquiryResponse>
            {
                Items = dtos,
                TotalCount = paged.TotalCount,
                Page = page,
                PageSize = size,
            }
        );
    }
}
