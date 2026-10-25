using MediatR;
using Patient.Application.Appointments.Dtos;

namespace Patient.Application.Appointments.Queries.GetPatientAppointments;

public record GetPatientAppointmentsQuery(Guid PatientId) : IRequest<List<AppointmentDto>>;