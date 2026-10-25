using MediatR;
using Microsoft.EntityFrameworkCore;
using Patient.Application.Common.Interfaces;

namespace Patient.Application.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Unit>
{
    private readonly IPatientDbContext _context;

    public CancelAppointmentCommandHandler(IPatientDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        // 1. Find the appointment by ID
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

        if (appointment == null)
        {
            throw new KeyNotFoundException($"Appointment with ID {request.AppointmentId} was not found.");
        }

        // 2. Update the appointment status to Cancelled (Assuming you have a method or property for status)
        appointment.Cancel(); //  Domain Entity 

        // 3. Save changes to the database
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}