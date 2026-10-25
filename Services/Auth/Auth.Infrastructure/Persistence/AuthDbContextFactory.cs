using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Auth.Infrastructure.Persistence;

// Factory implementation to allow EF Core CLI tools to instantiate AuthDbContext at design time
public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        // Build DbContextOptions pointing to the local database for migrations
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

        // Define the fallback connection string used specifically during design-time migrations
        var connectionString = "Server=.;Database=HealthcareAuthDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);

        return new AuthDbContext(optionsBuilder.Options);
    }
}