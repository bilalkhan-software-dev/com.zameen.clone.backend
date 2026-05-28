using com.zameen.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace com.zameen.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Agent> Agents => Set<Agent>();
        public DbSet<Enquiry> Enquiries => Set<Enquiry>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Always call the base method first when inheriting from IdentityDbContext
            base.OnModelCreating(builder);

            // Map Property Enums to Strings in the DB
            builder.Entity<Property>().Property(p => p.Status).HasConversion<string>();

            builder.Entity<Property>().Property(p => p.PropertyType).HasConversion<string>();

            builder.Entity<Property>().Property(p => p.AreaUnit).HasConversion<string>();

            builder
                .Entity<ApplicationUser>()
                .Property(p => p.AccountStatus)
                .HasConversion<string>();
        }
    }
}
