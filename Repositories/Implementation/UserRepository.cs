using com.zameen.Data;
using com.zameen.Models;
using com.zameen.Repositories.Interfaces;

namespace com.zameen.Repositories.Implementation
{
    public class UserRepository(ApplicationDbContext context)
        : GenericRepository<ApplicationUser, Guid>(context),
            IUserRepository
    {
        private readonly ApplicationDbContext _context = context;
    }
}
