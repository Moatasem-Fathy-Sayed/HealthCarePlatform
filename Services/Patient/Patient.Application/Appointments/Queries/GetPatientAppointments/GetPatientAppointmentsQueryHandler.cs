using MediatR;
using Microsoft.EntityFrameworkCore;
using Patient.Application.Common.Interfaces;
using Patient.Application.Appointments.Dtos;

namespace Patient.Application.Appointments.Queries.GetPatientAppointments;

public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, List<AppointmentDto>>
{
    private readonly IPatientDbContext _context;

    public GetPatientAppointmentsQueryHandler(IPatientDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentDto>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var appointments = await _context.Appointments
            .Where(a => a.PatientId == request.PatientId)
            .Select(a => new AppointmentDto(
                a.Id,
                a.PatientId,
                a.DoctorId,
                a.AppointmentDate,
                a.ReasonForVisit,
                a.Status.ToString()
            ))
            .ToListAsync(cancellationToken);

        return appointments;
    }
}