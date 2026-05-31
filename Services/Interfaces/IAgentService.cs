using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;

namespace com.zameen.Services.Interfaces;

public interface IAgentService
{
    Task<ApiResponse<AgentResponse>> CreateAgentAsync(RegisterRequest request, string userId);
    Task<ApiResponse<AgentResponse>> GetAgentByUserIdAsync(string userId);
    Task<ApiResponse<AgentResponse>> GetAgentByIdAsync(string agentId);
    Task<ApiResponse<AgentResponse>> UpdateAgentAsync(string userId, UpdateAgentRequest request);
    Task<ApiResponse> DeleteAgentAsync(string agentId, string requestingUserId); // admin only?
    Task<ApiResponse<PagedResult<AgentResponse>>> GetAgentsAsync(
        int page,
        int size,
        AccountStatus? statusFilter,
        string? sortBy,
        bool isDescending
    );
    Task<ApiResponse> ApproveAgentAsync(string agentId);
    Task<ApiResponse> RejectAgentAsync(string agentId);
}
