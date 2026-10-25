using MediatR;
using Microsoft.AspNetCore.Mvc;
using Patient.Application.Appointments.Commands;
using Patient.Application.Appointments.Commands.CancelAppointment;
using Patient.Application.Appointments.Queries.GetPatientAppointments;
using Patient.Application.Patients.Commands.BookAppointment;

namespace Patient.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Books a new medical appointment.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> BookAppointment([FromBody] BookAppointmentCommand command, CancellationToken cancellationToken)
    {
        var appointmentId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(BookAppointment), new { id = appointmentId }, appointmentId);
    }

    /// <summary>
    /// Retrieves all appointments for a specific patient.
    /// </summary>
    [HttpGet("patient/{patientId:guid}")]
    public async Task<IActionResult> GetPatientAppointments(Guid patientId, CancellationToken cancellationToken)
    {
        var query = new GetPatientAppointmentsQuery(patientId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Cancels an existing appointment.
    /// </summary>
    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelAppointmentCommand(id);
        await _mediator.Send(command, cancellationToken);

        return NoContent(); // Return 204 No Content on successful cancellation
    }
}