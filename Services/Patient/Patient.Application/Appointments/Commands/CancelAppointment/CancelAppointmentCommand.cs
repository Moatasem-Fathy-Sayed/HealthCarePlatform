using MediatR;

namespace Patient.Application.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(Guid AppointmentId) : IRequest<Unit>;