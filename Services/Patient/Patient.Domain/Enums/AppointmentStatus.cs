namespace Patient.Domain.Enums;

/// <summary>
/// Represents the life cycle status of an appointment.
/// </summary>
public enum AppointmentStatus
{
    Scheduled = 1,
    Completed = 2,
    Cancelled = 3,
    NoShow = 4
}