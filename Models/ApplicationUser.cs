using com.zameen.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace com.zameen.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public AccountStatus AccountStatus { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = [];

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
