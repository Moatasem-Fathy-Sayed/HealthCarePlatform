namespace Patient.Application.Appointments.Dtos;

public record AppointmentDto(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    DateTime AppointmentDate,
    string ReasonForVisit,
    string Status
);