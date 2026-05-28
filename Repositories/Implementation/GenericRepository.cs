using com.zameen.Data;
using com.zameen.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace com.zameen.Repositories.Implementation
{
    public class GenericRepository<T, ID> : IGenericRepository<T, ID>
        where T : class
    {
        private readonly ApplicationDbContext _context;
        public readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> GetByIdAsync(ID id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            int page,
            int size,
            string sortBy,
            bool isNewest
        )
        {
            var query = _dbSet.AsQueryable().AsNoTracking();

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = isNewest
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }

            // Pagination
            return await query.Skip((page - 1) * size).Take(size).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        //  Update Tracking in EF Core State
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        //  Update Tracking in EF Core State
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
