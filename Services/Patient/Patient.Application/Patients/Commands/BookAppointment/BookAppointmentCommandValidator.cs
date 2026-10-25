using FluentValidation;

namespace Patient.Application.Patients.Commands.BookAppointment;

public class BookAppointmentCommandValidator : AbstractValidator<BookAppointmentCommand>
{
    public BookAppointmentCommandValidator()
    {
        // 1. Ensure PatientId is not empty (valid Guid)
        RuleFor(v => v.PatientId)
            .NotEmpty().WithMessage("Patient ID is required.");

        // 2. Ensure DoctorId is not empty
        RuleFor(v => v.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required.");

        // 3. Ensure appointment date is in the future
        RuleFor(v => v.AppointmentDate)
            .NotEmpty().WithMessage("Appointment date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future.");

        // 4. Ensure reason for visit is provided and within length limit
        RuleFor(v => v.ReasonForVisit)
            .NotEmpty().WithMessage("Reason for visit is required.")
            .MaximumLength(500).WithMessage("Reason for visit must not exceed 500 characters.");
    }
}