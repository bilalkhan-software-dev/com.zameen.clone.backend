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
        public DbSet<SearchLog> SearchLogs => Set<SearchLog>();
        public DbSet<PriceTrend> PriceTrends => Set<PriceTrend>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Always call the base method first when inheriting from IdentityDbContext
            base.OnModelCreating(builder);

            // Map Property Enums to Strings in the DB
            builder.Entity<Property>().Property(p => p.Status).HasConversion<string>();
            builder.Entity<Property>().Property(p => p.PropertyType).HasConversion<string>();
            builder.Entity<Property>().Property(p => p.PropertyPurpose).HasConversion<string>();
            builder.Entity<Property>().Property(p => p.Price).HasPrecision(18, 2);
            builder.Entity<Property>().Property(p => p.AreaSize).HasPrecision(18, 2);
            builder.Entity<Property>().Property(p => p.Longitude).HasPrecision(18, 7);
            builder.Entity<Property>().Property(p => p.Latitude).HasPrecision(18, 7);
            builder.Entity<Property>().HasIndex(p => p.City).HasDatabaseName("IX_Property_City");
            builder
                .Entity<Property>()
                .HasIndex(p => new { p.City, p.Location })
                .HasFilter("[IsActive] = 1 AND [Location] IS NOT NULL")
                .HasDatabaseName("IX_Property_City_Location_Active");
            builder
                .Entity<Property>()
                .HasIndex(p => p.Location)
                .HasDatabaseName("IX_Property_Location");
            builder
                .Entity<Property>()
                .HasIndex(p => p.AgentId)
                .HasDatabaseName("IX_Property_AgentId");

            builder.Entity<PriceTrend>().Property(p => p.PropertyType).HasConversion<string>();
            builder.Entity<PriceTrend>().Property(p => p.PropertyPurpose).HasConversion<string>();
            builder.Entity<PriceTrend>().Property(pt => pt.AveragePrice).HasPrecision(18, 2);

            builder.Entity<Agent>().Property(a => a.AccountStatus).HasConversion<string>();
            builder.Entity<Agent>().HasIndex(u => u.UserId).HasDatabaseName("IX_Agent_UserId");

            builder
                .Entity<ApplicationUser>()
                .Property(p => p.AccountStatus)
                .HasConversion<string>();
        }
    }
}
