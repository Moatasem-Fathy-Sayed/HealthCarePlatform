using Patient.Domain.Enums;

namespace Patient.Domain.Entities;

/// <summary>
/// Represents an appointment entity within the healthcare system.
/// </summary>
public class Appointment
{
    public Guid Id { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTime AppointmentDate { get; private set; }
    public string ReasonForVisit { get; private set; } = string.Empty;
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Private constructor required by Entity Framework Core.
    /// </summary>
    private Appointment() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Appointment"/> class with domain validation rules.
    /// </summary>
    public Appointment(Guid patientId, Guid doctorId, DateTime appointmentDate, string reasonForVisit)
    {
        if (appointmentDate <= DateTime.UtcNow)
        {
            throw new ArgumentException("Appointment date must be in the future.", nameof(appointmentDate));
        }

        Id = Guid.NewGuid();
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentDate = appointmentDate;
        ReasonForVisit = reasonForVisit;
        Status = AppointmentStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the scheduled appointment.
    /// </summary>
    public void Cancel()
    {
        if (Status == AppointmentStatus.Completed)
        {
            throw new InvalidOperationException("Cannot cancel a completed appointment.");
        }

        Status = AppointmentStatus.Cancelled;
    }

    /// <summary>
    /// Marks the appointment as completed after a successful visit.
    /// </summary>
    public void Complete()
    {
        if (Status != AppointmentStatus.Scheduled)
        {
            throw new InvalidOperationException("Only scheduled appointments can be completed.");
        }

        Status = AppointmentStatus.Completed;
    }
}