namespace com.zameen.Repositories.Interfaces
{
    public interface IGenericRepository<T, ID>
        where T : class
    {
        Task<T> GetByIdAsync(ID id);
        Task<IEnumerable<T>> GetAllAsync(int page, int size, string sortBy, bool isNewet);
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}
