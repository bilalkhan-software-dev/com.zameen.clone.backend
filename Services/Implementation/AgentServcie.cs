using AutoMapper;
using com.zameen.Exceptions;
using com.zameen.Models;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;
using com.zameen.Repositories.Interfaces;
using com.zameen.Services.Interfaces;

namespace com.zameen.Services.Implementation;

public class AgentService(
    IAgentRepository _agentRepo,
    IMapper _mapper,
    ILogger<AgentService> _logger
) : IAgentService
{
    public async Task<ApiResponse<AgentResponse>> CreateAgentAsync(
        RegisterRequest request,
        string userId
    )
    {
        _logger.LogInformation(
            "Creating agent profile for agency '{AgencyName}'",
            request.AgencyName
        );

        var agent = new Agent
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            AgencyName = request.AgencyName!,
            Bio = request.Bio,
            AccountStatus = AccountStatus.PENDING,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _agentRepo.AddAsync(agent);
        _logger.LogInformation("Agent created with ID {AgentId}", agent.Id);

        var response = _mapper.Map<AgentResponse>(agent);
        return ApiResponse<AgentResponse>.Ok(response, "Agent profile created.");
    }

    public async Task<ApiResponse<AgentResponse>> GetAgentByUserIdAsync(string userId)
    {
        _logger.LogDebug("Fetching agent by UserId {UserId}", userId);
        var agent = await _agentRepo.GetByUserIdAsync(userId);
        if (agent == null)
            throw new ResourceNotFoundException("Agent profile not found.");

        return ApiResponse<AgentResponse>.Ok(_mapper.Map<AgentResponse>(agent));
    }

    public async Task<ApiResponse<AgentResponse>> GetAgentByIdAsync(string agentId)
    {
        _logger.LogDebug("Fetching agent by ID {AgentId}", agentId);
        var agent =
            await _agentRepo.GetByIdAsync(agentId)
            ?? throw new ResourceNotFoundException($"Agent with ID '{agentId}' not found.");
        return ApiResponse<AgentResponse>.Ok(_mapper.Map<AgentResponse>(agent));
    }

    public async Task<ApiResponse<AgentResponse>> UpdateAgentAsync(
        string userId,
        UpdateAgentRequest request
    )
    {
        _logger.LogInformation("Updating agent profile for user {UserId}", userId);
        var agent =
            await _agentRepo.GetByUserIdAsync(userId)
            ?? throw new ResourceNotFoundException("Agent profile not found.");
        _mapper.Map(request, agent);
        agent.UpdatedAt = DateTime.UtcNow;
        _agentRepo.Update(agent);
        await _agentRepo.SaveChangesAsync();

        _logger.LogInformation("Agent {AgentId} updated", agent.Id);
        return ApiResponse<AgentResponse>.Ok(_mapper.Map<AgentResponse>(agent), "Agent updated.");
    }

    public async Task<ApiResponse> DeleteAgentAsync(string agentId, string requestingUserId)
    {
        _logger.LogWarning(
            "Deleting agent {AgentId} by user {RequestingUserId}",
            agentId,
            requestingUserId
        );
        var agent = await _agentRepo.GetByIdAsync(agentId);
        if (agent == null)
            throw new ResourceNotFoundException("Agent not found.");

        _agentRepo.Delete(agent);
        await _agentRepo.SaveChangesAsync();

        _logger.LogInformation("Agent {AgentId} deleted", agentId);
        return ApiResponse.Ok("Agent deleted.");
    }

    public async Task<ApiResponse<PagedResult<AgentResponse>>> GetAgentsAsync(
        int page,
        int size,
        AccountStatus? statusFilter,
        string? sortBy,
        bool isDescending
    )
    {
        _logger.LogDebug(
            "Listing agents page {Page} size {Size} status {Status}",
            page,
            size,
            statusFilter
        );
        var paged = await _agentRepo.GetPagedAsync(page, size, statusFilter, sortBy, isDescending);
        var dtos = _mapper.Map<IEnumerable<AgentResponse>>(paged.Items);
        return ApiResponse<PagedResult<AgentResponse>>.Ok(
            new PagedResult<AgentResponse>
            {
                Items = dtos,
                TotalCount = paged.TotalCount,
                Page = paged.Page,
                PageSize = paged.PageSize,
            }
        );
    }

    public async Task<ApiResponse> ApproveAgentAsync(string agentId)
    {
        _logger.LogInformation("Approving agent {AgentId}", agentId);
        var agent = await _agentRepo.GetByIdAsync(agentId);
        if (agent == null)
            throw new ResourceNotFoundException("Agent not found.");

        if (agent.AccountStatus == AccountStatus.APPROVED)
            throw new ResourceAlreadyExistsException("Agent is already approved.");

        agent.AccountStatus = AccountStatus.APPROVED;
        agent.UpdatedAt = DateTime.UtcNow;
        _agentRepo.Update(agent);
        await _agentRepo.SaveChangesAsync();

        return ApiResponse.Ok("Agent approved.");
    }

    public async Task<ApiResponse> RejectAgentAsync(string agentId)
    {
        _logger.LogInformation("Rejecting agent {AgentId}", agentId);
        var agent = await _agentRepo.GetByIdAsync(agentId);
        if (agent == null)
            throw new ResourceNotFoundException("Agent not found.");

        agent.AccountStatus = AccountStatus.REJECTED;
        agent.UpdatedAt = DateTime.UtcNow;
        _agentRepo.Update(agent);
        await _agentRepo.SaveChangesAsync();

        return ApiResponse.Ok("Agent rejected.");
    }
}
