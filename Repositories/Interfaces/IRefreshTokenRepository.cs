using com.zameen.Models;

namespace com.zameen.Repositories.Interfaces;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken, int>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
}
