using com.zameen.Data;
using com.zameen.Models;
using com.zameen.Models.Dto.Response;
using com.zameen.Models.Enums;
using com.zameen.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace com.zameen.Repositories.Implementation;

public class AgentRepository(ApplicationDbContext context)
    : GenericRepository<Agent, string>(context),
        IAgentRepository
{
    public async Task<Agent?> GetByUserIdAsync(string userId) =>
        await _dbSet.FirstOrDefaultAsync(a => a.UserId == userId);

    public async Task<PagedResult<Agent>> GetPagedAsync(
        int page,
        int size,
        AccountStatus? statusFilter,
        string? sortBy,
        bool isDescending
    )
    {
        var query = _dbSet.AsQueryable().AsNoTracking();

        if (statusFilter.HasValue)
            query = query.Where(a => a.AccountStatus == statusFilter.Value);

        if (string.IsNullOrEmpty(sortBy))
            sortBy = "CreatedAt";

        query = isDescending
            ? query.OrderByDescending(e => EF.Property<object>(e, sortBy!))
            : query.OrderBy(e => EF.Property<object>(e, sortBy!));

        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<Agent>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = size,
        };
    }
}
