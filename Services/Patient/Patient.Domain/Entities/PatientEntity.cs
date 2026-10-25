using Patient.Domain.Enums;

namespace Patient.Domain.Entities;

public class PatientEntity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } // Reference to Auth.API User Account
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string NationalId { get; private set; } = string.Empty;
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public BloodGroup BloodGroup { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string? EmergencyContactName { get; private set; }
    public string? EmergencyContactPhone { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private PatientEntity() { } // Required for EF Core

    public PatientEntity(
        Guid userId,
        string firstName,
        string lastName,
        string nationalId,
        DateTime dateOfBirth,
        Gender gender,
        BloodGroup bloodGroup,
        string phoneNumber,
        string address,
        string? emergencyContactName = null,
        string? emergencyContactPhone = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        NationalId = nationalId;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        BloodGroup = bloodGroup;
        PhoneNumber = phoneNumber;
        Address = address;
        EmergencyContactName = emergencyContactName;
        EmergencyContactPhone = emergencyContactPhone;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        string phoneNumber,
        string address,
        BloodGroup bloodGroup,
        string? emergencyContactName,
        string? emergencyContactPhone)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Address = address;
        BloodGroup = bloodGroup;
        EmergencyContactName = emergencyContactName;
        EmergencyContactPhone = emergencyContactPhone;
        UpdatedAtUtc = DateTime.UtcNow;
    }

}