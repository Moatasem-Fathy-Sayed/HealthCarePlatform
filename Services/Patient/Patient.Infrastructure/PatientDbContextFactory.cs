using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Patient.Infrastructure.Persistence;

namespace Patient.Infrastructure;

public class PatientDbContextFactory : IDesignTimeDbContextFactory<PatientDbContext>
{
    public PatientDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PatientDbContext>();

        // Connection string directly used for migration generation
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PatientDb;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new PatientDbContext(optionsBuilder.Options);
    }
}