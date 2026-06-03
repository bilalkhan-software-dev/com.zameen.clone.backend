using com.zameen.Models;
using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;

namespace com.zameen.Repositories.Interfaces;

public interface IAgentRepository : IGenericRepository<Agent, string>
{
    Task<Agent?> GetByUserIdAsync(string userId);
    Task<PagedResult<Agent>> GetPagedAsync(
        int page,
        int size,
        AccountStatus? statusFilter,
        string? sortBy,
        bool isDescending
    );
}
