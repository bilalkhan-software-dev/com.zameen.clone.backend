using com.zameen.Data;
using com.zameen.Models;
using com.zameen.Models.Dto.Response;
using com.zameen.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace com.zameen.Repositories.Implementation;

public class EnquiryRepository(ApplicationDbContext context)
    : GenericRepository<Enquiry, int>(context),
        IEnquiryRepository
{
    public async Task<PagedResult<Enquiry>> GetByPropertyIdAsync(int propertyId, int page, int size)
    {
        var query = _dbSet
            .Where(e => e.PropertyId == propertyId)
            .OrderByDescending(e => e.CreatedAt);

        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<Enquiry>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = size,
        };
    }

    public async Task<PagedResult<Enquiry>> GetAllEnquiryByAgentAsync(
        string agentId,
        int page,
        int size
    )
    {
        var query = _dbSet.Where(e => e.AgentId == agentId).OrderByDescending(e => e.CreatedAt);

        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<Enquiry>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = size,
        };
    }
}
