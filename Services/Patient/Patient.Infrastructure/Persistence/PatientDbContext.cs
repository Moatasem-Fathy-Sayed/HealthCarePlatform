using Microsoft.EntityFrameworkCore;
using Patient.Application.Common.Interfaces;
using Patient.Domain.Entities;

namespace Patient.Infrastructure.Persistence;

/// <summary>
/// DbContext implementation for Patient Microservice.
/// </summary>
public class PatientDbContext : DbContext, IPatientDbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options)
    {
    }

    public DbSet<PatientEntity> Patients => Set<PatientEntity>();

    // DbSet for appointments to fulfill IPatientDbContext requirements
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PatientDbContext).Assembly);
    }
}