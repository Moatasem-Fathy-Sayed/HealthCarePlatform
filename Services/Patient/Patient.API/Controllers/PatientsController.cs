using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Patient.Application.Patients.Commands.CreatePatient;
using Patient.Application.Patients.Commands.UpdatePatient;
using Patient.Application.Patients.Dtos;
using Patient.Application.Patients.Queries.GetMyProfile;

namespace Patient.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class PatientsController : ControllerBase
{
    private readonly ISender _mediator;

    public PatientsController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new patient record in the system.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves the patient profile for the currently authenticated user.
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var query = new GetMyProfileQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Updates patient profile details for the currently authenticated user.
    /// </summary>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdatePatientCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}