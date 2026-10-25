namespace Patient.Application.Patients.Dtos;

/// <summary>
/// Data Transfer Object representing patient details returned to the API client.
/// </summary>
public record PatientDto(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    string NationalId,
    DateTime DateOfBirth,
    string Gender,
    string BloodGroup,
    string PhoneNumber,
    string Address,
    string? EmergencyContactName,
    string? EmergencyContactPhone
);