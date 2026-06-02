using AutoMapper;
using com.zameen.Exceptions;
using com.zameen.Models;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;
using com.zameen.Repositories.Implementation;
using com.zameen.Repositories.Interfaces;
using com.zameen.Services.Interfaces;

namespace com.zameen.Services.Implementation;

public class PropertyService(
    IPropertyRepository propertyRepo,
    IAgentRepository agentRepo,
    IMapper mapper,
    ILogger<PropertyService> logger
) : IPropertyService
{
    private readonly IPropertyRepository _propertyRepo = propertyRepo;
    private readonly IAgentRepository _agentRepo = agentRepo;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<PropertyService> _logger = logger;

    public async Task<ApiResponse<PagedResult<PropertyResponse>>> SearchAsync(
        PropertyFilterParams filters
    )
    {
        _logger.LogInformation(
            "Property search: City={City}, Type={Type}, Price={Min}-{Max}",
            filters.City,
            filters.PropertyType,
            filters.MinPrice,
            filters.MaxPrice
        );

        var paged = await _propertyRepo.SearchAsync(filters);
        var items = _mapper.Map<IEnumerable<PropertyResponse>>(paged.Items);

        // Enrich with agent name
        foreach (var item in items)
        {
            var agent = await _agentRepo.GetByIdAsync(item.AgentId);
            item.AgentName = agent?.AgencyName ?? "";
        }

        return ApiResponse<PagedResult<PropertyResponse>>.Ok(
            new PagedResult<PropertyResponse>
            {
                Items = items,
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize,
            }
        );
    }

    public async Task<ApiResponse<PropertyResponse>> GetByIdAsync(int id)
    {
        _logger.LogDebug("Fetching property ID {PropertyId}", id);
        var property = await _propertyRepo.GetByIdAsync(id);
        if (property == null || !property.IsActive)
            throw new ResourceNotFoundException($"Property with ID {id} not found.");

        var agent = await _agentRepo.GetByIdAsync(property.AgentId);
        var response = _mapper.Map<PropertyResponse>(property);
        response.AgentName = agent?.AgencyName ?? "";
        return ApiResponse<PropertyResponse>.Ok(response);
    }

    public async Task<ApiResponse<PropertyResponse>> CreateAsync(
        CreatePropertyRequest request,
        string agentUserId
    )
    {
        _logger.LogInformation("Creating property for user {UserId}", agentUserId);

        var agent = await _agentRepo.GetByUserIdAsync(agentUserId);
        if (agent == null || agent.AccountStatus != AccountStatus.APPROVED)
            return ApiResponse<PropertyResponse>.Fail("Agent not found or not approved.");

        if (await _propertyRepo.ExistsByTitleAsync(request.Title))
            throw new ResourceAlreadyExistsException("A property with this title already exists.");

        var property = _mapper.Map<Property>(request);
        property.AgentId = agent.Id; // ← fix here
        property.PropertyPics = request.PropertyPics ?? [];
        property.Status = PropertyStatus.PENDING; // set a default status
        property.IsActive = true;

        await _propertyRepo.AddAsync(property);
        // AddAsync already calls SaveChangesAsync, but we can call again just in case
        // await _propertyRepo.SaveChangesAsync();

        _logger.LogInformation(
            "Property {PropertyId} created by agent {AgentId}",
            property.Id,
            agent.Id
        );

        var response = _mapper.Map<PropertyResponse>(property);
        response.AgentName = agent.AgencyName;
        return ApiResponse<PropertyResponse>.Ok(response, "Property created.");
    }

    public async Task<ApiResponse<PropertyResponse>> UpdateAsync(
        int id,
        UpdatePropertyRequest request,
        string agentUserId
    )
    {
        _logger.LogInformation("Updating property {PropertyId} by agent {UserId}", id, agentUserId);

        var property = await _propertyRepo.GetByIdAsync(id);
        if (property == null || !property.IsActive)
            throw new ResourceNotFoundException("Property not found.");

        var agent = await _agentRepo.GetByUserIdAsync(agentUserId);
        if (agent == null || property.AgentId != agent.Id)
            return ApiResponse<PropertyResponse>.Fail(
                "You don't have permission to update this property."
            );

        // Check duplicate title only if title is being changed
        if (!string.IsNullOrWhiteSpace(request.Title) && request.Title != property.Title)
        {
            bool duplicate = await _propertyRepo.ExistsByTitleAsync(request.Title, id);
            if (duplicate)
                throw new ResourceAlreadyExistsException(
                    "Another property with this title already exists."
                );
        }

        // Apply only the fields that are not null
        if (request.Title is not null)
            property.Title = request.Title;
        if (request.Description is not null)
            property.Description = request.Description;
        if (request.Price.HasValue)
            property.Price = request.Price.Value;
        if (request.City is not null)
            property.City = request.City;
        if (request.Address is not null)
            property.Address = request.Address;
        if (request.Bedrooms.HasValue)
            property.Bedrooms = request.Bedrooms.Value;
        if (request.Bathrooms.HasValue)
            property.Bathrooms = request.Bathrooms.Value;
        if (request.AreaSize.HasValue)
            property.AreaSize = request.AreaSize.Value;
        if (request.AreaUnit.HasValue)
            property.AreaUnit = request.AreaUnit.Value;
        if (request.PropertyType.HasValue)
            property.PropertyType = request.PropertyType.Value;
        if (request.PropertyPics != null && request.PropertyPics.Count > 0)
        {
            property.PropertyPics = request.PropertyPics;
        }

        property.UpdatedAt = DateTime.UtcNow;
        _propertyRepo.Update(property);
        await _propertyRepo.SaveChangesAsync();

        var response = _mapper.Map<PropertyResponse>(property);
        response.AgentName = agent.AgencyName;
        _logger.LogInformation("Property {PropertyId} updated", id);
        return ApiResponse<PropertyResponse>.Ok(response, "Property updated.");
    }

    public async Task<ApiResponse> DeleteAsync(int id, string agentUserId)
    {
        _logger.LogWarning("Deleting property {PropertyId} by user {UserId}", id, agentUserId);

        var property = await _propertyRepo.GetByIdAsync(id);
        if (property == null)
            throw new ResourceNotFoundException("Property not found.");

        var agent = await _agentRepo.GetByUserIdAsync(agentUserId);
        if (agent == null || property.AgentId != agent.Id)
            return ApiResponse.Fail("You do not have permission to delete this property.");

        property.IsActive = false;
        property.UpdatedAt = DateTime.UtcNow;
        _propertyRepo.Update(property);
        await _propertyRepo.SaveChangesAsync();

        _logger.LogInformation("Property {PropertyId} deactivated", id);
        return ApiResponse.Ok("Property deactivated.");
    }

    public async Task<ApiResponse<PagedResult<PropertyResponse>>> GetPropertiesByAgentAsync(
        string agentUserId,
        int page,
        int size
    )
    {
        _logger.LogDebug("Fetching properties for agent user {UserId}", agentUserId);
        var agent = await _agentRepo.GetByUserIdAsync(agentUserId);
        if (agent == null)
            throw new ResourceNotFoundException("Agent not found.");

        var paged = await _propertyRepo.GetPropertiesByAgentAsync(agentUserId, page, size);
        var dtos = _mapper.Map<IEnumerable<PropertyResponse>>(paged.Items);
        foreach (var dto in dtos)
            dto.AgentName = agent.AgencyName;

        return ApiResponse<PagedResult<PropertyResponse>>.Ok(
            new PagedResult<PropertyResponse>
            {
                Items = dtos,
                TotalCount = paged.TotalCount,
                Page = page,
                PageSize = size,
            }
        );
    }

    public async Task<ApiResponse> ToggleActiveAsync(int id, string agentUserId)
    {
        _logger.LogInformation("Toggling active status for property {PropertyId}", id);

        var property = await _propertyRepo.GetByIdAsync(id);
        if (property == null)
            throw new ResourceNotFoundException("Property not found.");

        var agent = await _agentRepo.GetByUserIdAsync(agentUserId);
        if (agent == null || property.AgentId != agent.Id)
            return ApiResponse.Fail("Permission denied.");

        property.IsActive = !property.IsActive;
        property.UpdatedAt = DateTime.UtcNow;
        _propertyRepo.Update(property);
        await _propertyRepo.SaveChangesAsync();

        string status = property.IsActive ? "activated" : "deactivated";
        _logger.LogInformation("Property {PropertyId} {Status}", id, status);
        return ApiResponse.Ok($"Property {status}.");
    }

    public async Task<ApiResponse<PagedResult<PropertyResponse>>> GetAllProperties(
        int page,
        int size,
        string sortBy,
        bool isNewest
    )
    {
        var paged = await _propertyRepo.GetPagedAsync(page, size, sortBy, isNewest);
        var dtos = _mapper.Map<IEnumerable<PropertyResponse>>(paged.Items);

        return ApiResponse<PagedResult<PropertyResponse>>.Ok(
            new PagedResult<PropertyResponse>
            {
                Items = dtos,
                TotalCount = paged.TotalCount,
                Page = page,
                PageSize = size,
            }
        );
    }

    public async Task<ApiResponse> UpdatePropertyStatus(int id, PropertyStatus propertyStatus)
    {
        _logger.LogInformation("Update Property status for property {PropertyId}", id);

        var property =
            await _propertyRepo.GetByIdAsync(id)
            ?? throw new ResourceNotFoundException("Property not found.");

        if (property.Status.Equals(propertyStatus))
            return ApiResponse.Ok("Property status is already " + propertyStatus);

        property.Status = propertyStatus;
        _propertyRepo.Update(property);
        await _propertyRepo.SaveChangesAsync();
        return ApiResponse.Ok($"Property with status {propertyStatus} update successfully.");
    }
}
