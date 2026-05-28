using com.zameen.Data;
using com.zameen.Models;
using com.zameen.Repositories.Interfaces;

namespace com.zameen.Repositories.Implementation
{
    public class UserRepository : GenericRepository<ApplicationUser, Guid>, IUserRepository
    {

        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
