using Microsoft.EntityFrameworkCore;
using Patient.Domain.Entities;

namespace Patient.Application.Common.Interfaces;

/// <summary>
/// Interface representing the patient and appointment database context.
/// </summary>
public interface IPatientDbContext
{
    /// <summary>
    /// Gets the DbSet for patient profiles.
    /// </summary>
    DbSet<PatientEntity> Patients { get; }

    /// <summary>
    /// Gets the DbSet for medical appointments.
    /// </summary>
    DbSet<Appointment> Appointments { get; }

    /// <summary>
    /// Saves all changes made in this context to the database asynchronously.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}