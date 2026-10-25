using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.Persistence;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        var roles = new[]
        {
            UserType.Patient.ToString(),
            UserType.Doctor.ToString(),
            UserType.Admin.ToString(),
            UserType.Staff.ToString()
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Name = role,
                    NormalizedName = role.ToUpperInvariant()
                });
            }
        }
    }
}