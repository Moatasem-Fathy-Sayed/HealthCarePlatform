using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Patient.Application.Patients.Commands.BookAppointment;

public record BookAppointmentCommand(
Guid PatientId,
Guid DoctorId,
DateTime AppointmentDate,
string ReasonForVisit
) : IRequest<Guid>;
