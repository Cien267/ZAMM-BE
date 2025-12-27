using Microsoft.EntityFrameworkCore;
using Zamm.Domain.Entities;
using Zamm.Infrastructure.DataContext;

namespace Zamm.Data;

public class DbInitializer
{
    public static async Task SeedRoles(AppDbContext context)
    {
        if (await context.Roles.AnyAsync())
        {
            return;
        }

        var roles = new List<Role>
        {
            new Role { RoleCode = "User", RoleName = "User", CreatedAt = DateTime.UtcNow },
            new Role { RoleCode = "Admin", RoleName = "Administrator", CreatedAt = DateTime.UtcNow },
            new Role { RoleCode = "Moderator", RoleName = "Moderator", CreatedAt = DateTime.UtcNow }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }
}