using FluentValidation;

namespace Patient.Application.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandValidator : AbstractValidator<CancelAppointmentCommand>
{
    public CancelAppointmentCommandValidator()
    {
        // Ensure AppointmentId is not empty
        RuleFor(v => v.AppointmentId)
            .NotEmpty().WithMessage("Appointment ID is required.");
    }
}