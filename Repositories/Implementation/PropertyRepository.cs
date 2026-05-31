using com.zameen.Data;
using com.zameen.Models;
using com.zameen.Models.Dto.Request;
using com.zameen.Models.Dto.Response;
using com.zameen.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace com.zameen.Repositories.Implementation;

public class PropertyRepository(ApplicationDbContext context) : GenericRepository<Property, int>(context), IPropertyRepository
{
    public async Task<PagedResult<Property>> SearchAsync(PropertyFilterParams filters)
    {
        var query = _dbSet.AsQueryable().AsNoTracking().Where(p => p.IsActive); // only active properties by default

        if (!string.IsNullOrWhiteSpace(filters.City))
            query = query.Where(p => p.City == filters.City);
        if (filters.PropertyType.HasValue)
            query = query.Where(p => p.PropertyType == filters.PropertyType.Value);
        if (filters.Status.HasValue)
            query = query.Where(p => p.Status == filters.Status.Value);
        if (filters.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filters.MinPrice.Value);
        if (filters.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filters.MaxPrice.Value);
        if (filters.MinBedrooms.HasValue)
            query = query.Where(p => p.Bedrooms >= filters.MinBedrooms.Value);
        if (filters.MaxBedrooms.HasValue)
            query = query.Where(p => p.Bedrooms <= filters.MaxBedrooms.Value);
        if (filters.MinAreaSize.HasValue)
            query = query.Where(p => p.AreaSize >= filters.MinAreaSize.Value);
        if (filters.MaxAreaSize.HasValue)
            query = query.Where(p => p.AreaSize <= filters.MaxAreaSize.Value);
        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            var term = filters.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(term) || p.Description.ToLower().Contains(term)
            );
        }

        // Sorting
        IOrderedQueryable<Property> orderedQuery;
        switch (filters.SortBy?.ToLower())
        {
            case "price":
                orderedQuery = filters.IsDescending
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price);
                break;
            case "areasize":
                orderedQuery = filters.IsDescending
                    ? query.OrderByDescending(p => p.AreaSize)
                    : query.OrderBy(p => p.AreaSize);
                break;
            case "bedrooms":
                orderedQuery = filters.IsDescending
                    ? query.OrderByDescending(p => p.Bedrooms)
                    : query.OrderBy(p => p.Bedrooms);
                break;
            default:
                orderedQuery = filters.IsDescending
                    ? query.OrderByDescending(p => p.CreatedAt)
                    : query.OrderBy(p => p.CreatedAt);
                break;
        }

        int total = await orderedQuery.CountAsync();
        var items = await orderedQuery
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync();

        return new PagedResult<Property>
        {
            Items = items,
            TotalCount = total,
            Page = filters.Page,
            PageSize = filters.PageSize,
        };
    }

    public async Task<PagedResult<Property>> GetPropertiesByAgentAsync(
        string agentId,
        int page,
        int size
    )
    {
        var query = _dbSet
            .Where(p => p.AgentId == agentId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt);

        int total = await query.CountAsync();
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

        return new PagedResult<Property>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = size,
        };
    }

    public async Task<bool> ExistsByTitleAsync(string title, int? excludeId = null)
    {
        var query = _dbSet.Where(p => p.Title == title && p.IsActive);
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
