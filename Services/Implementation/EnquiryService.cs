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
    private readonly IEnquiryRepository _enquiryRepo = enquiryRepo;
    private readonly IPropertyRepository _propertyRepo = propertyRepo;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<EnquiryService> _logger = logger;

    public async Task<ApiResponse<EnquiryResponse>> SendEnquiryAsync(CreateEnquiryRequest request)
    {
        _logger.LogInformation(
            "Enquiry from {Email} for property {PropertyId}",
            request.SenderEmail,
            request.PropertyId
        );

        var property = await _propertyRepo.GetByIdAsync(request.PropertyId);
        if (property == null || !property.IsActive)
            throw new ResourceNotFoundException("Property not available.");

        var enquiry = _mapper.Map<Enquiry>(request);
        enquiry.CreatedAt = DateTime.UtcNow;
        await _enquiryRepo.AddAsync(enquiry);
        _logger.LogInformation("Enquiry created ID {EnquiryId}", enquiry.Id);

        return ApiResponse<EnquiryResponse>.Ok(
            _mapper.Map<EnquiryResponse>(enquiry),
            "Enquiry submitted."
        );
    }

    public async Task<ApiResponse<EnquiryResponse>> GetEnquiryByIdAsync(int id)
    {
        var enquiry = await _enquiryRepo.GetByIdAsync(id);
        if (enquiry == null)
            throw new ResourceNotFoundException("Enquiry not found.");

        return ApiResponse<EnquiryResponse>.Ok(_mapper.Map<EnquiryResponse>(enquiry));
    }

    public async Task<ApiResponse<PagedResult<EnquiryResponse>>> GetEnquiriesForPropertyAsync(
        int propertyId,
        int page,
        int size
    )
    {
        var paged = await _enquiryRepo.GetByPropertyIdAsync(propertyId, page, size);
        var dtos = _mapper.Map<IEnumerable<EnquiryResponse>>(paged.Items);

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
        _logger.LogWarning("Deleting enquiry {EnquiryId} by admin {AdminUserId}", id, adminUserId);

        var enquiry = await _enquiryRepo.GetByIdAsync(id);
        if (enquiry == null)
            throw new ResourceNotFoundException("Enquiry not found.");

        _enquiryRepo.Delete(enquiry);
        await _enquiryRepo.SaveChangesAsync();
        _logger.LogInformation("Enquiry {EnquiryId} deleted", id);
        return ApiResponse.Ok("Enquiry deleted.");
    }
}
