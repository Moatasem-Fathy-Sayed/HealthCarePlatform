using Healthcare.SharedKernel.Common;
using MediatR;
using Patient.Application.Patients.Dtos;

namespace Patient.Application.Patients.Queries.GetPatientById;

/// <summary>
/// Query request to fetch a specific patient by their unique identifier.
/// </summary>
public record GetPatientByIdQuery(Guid Id) : IRequest<Result<PatientDto>>;