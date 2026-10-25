using MediatR;
using Patient.Application.Common.Interfaces;
using Patient.Application.Patients.Commands.BookAppointment;
using Patient.Domain.Entities;

namespace Patient.Application.Appointments.Commands;

/// <summary>
/// Handler for processing BookAppointmentCommand and persisting the appointment to the database.
/// </summary>
public class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, Guid>
{
    private readonly IPatientDbContext _context;

    public BookAppointmentCommandHandler(IPatientDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        // 1. Instantiate the Appointment domain entity (applies domain rules & validations)
        var appointment = new Appointment(
            request.PatientId,
            request.DoctorId,
            request.AppointmentDate,
            request.ReasonForVisit
        );

        // 2. Add entity to the DbContext & save changes to the database
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync(cancellationToken);

        return appointment.Id;
    }
}