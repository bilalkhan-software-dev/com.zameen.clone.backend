using com.zameen.Data;
using com.zameen.Models;
using com.zameen.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace com.zameen.Repositories.Implementation;

public class RefreshTokenRepository(ApplicationDbContext context)
    : GenericRepository<RefreshToken, int>(context),
        IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
    }
}
