using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Patient.Domain.Entities;

namespace Patient.Infrastructure.Persistence.Configurations;

// Fluent API configurations for Patient entity
public class PatientConfiguration : IEntityTypeConfiguration<PatientEntity>
{
    public void Configure(EntityTypeBuilder<PatientEntity> builder)
    {
        // Set Primary Key
        builder.HasKey(p => p.Id);

        // Required fields with maximum lengths
        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.NationalId)
            .IsRequired()
            .HasMaxLength(14);

        // Ensure NationalId is unique across the system
        builder.HasIndex(p => p.NationalId)
            .IsUnique();

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(p => p.Address)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.EmergencyContactName)
            .HasMaxLength(100);

        builder.Property(p => p.EmergencyContactPhone)
            .HasMaxLength(15);

        // Convert Enums to string representation in database
        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.BloodGroup)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}