namespace com.zameen.Data;

using com.zameen.Models;
using com.zameen.Models.Enums;
using com.zameen.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

public static class SeedData
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<
            RoleManager<IdentityRole<Guid>>
        >();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var agentRepository = scope.ServiceProvider.GetRequiredService<IAgentRepository>();

        //  Create roles if they don’t exist
        string[] roles = ["User", "Agent", "Admin"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        //  Create a default admin user if no admin exists
        var adminEmail = "admin@example.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Admin",
                AccountStatus = AccountStatus.APPROVED,
            };
            var result = await userManager.CreateAsync(admin, "Admin@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        //  Create a default agent user for testing
        var agentEmail = "agent@example.com";
        if (await userManager.FindByEmailAsync(agentEmail) == null)
        {
            var demoAgent = new ApplicationUser
            {
                UserName = agentEmail,
                Email = agentEmail,
                EmailConfirmed = true,
                FullName = "Demo Agent",
                AccountStatus = AccountStatus.APPROVED,
            };

            var result = await userManager.CreateAsync(demoAgent, "Agent@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demoAgent, "Agent");
            }
            var agent = new Agent
            {
                UserId = demoAgent.Id.ToString(),
                AgencyName = "Demo Agency",
                Bio = "This is Demo Agency Bio",
            };
            await agentRepository.AddAsync(agent);
        }

        // Create a default normal user
        var userEmail = "user@example.com";
        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var user = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true,
                FullName = "Demo User",
                AccountStatus = AccountStatus.APPROVED,
            };
            var result = await userManager.CreateAsync(user, "User@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}
