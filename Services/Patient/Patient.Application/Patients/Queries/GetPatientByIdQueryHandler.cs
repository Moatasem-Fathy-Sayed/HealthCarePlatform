using Healthcare.SharedKernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Patient.Application.Common.Interfaces;
using Patient.Application.Patients.Dtos;

namespace Patient.Application.Patients.Queries.GetPatientById;

/// <summary>
/// Handler responsible for executing the GetPatientByIdQuery request.
/// </summary>
public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    private readonly IPatientDbContext _context;

    public GetPatientByIdQueryHandler(IPatientDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        // 1. Fetch patient record from database by ID without tracking
        var patient = await _context.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        // 2. Return Failure result explicitly typed with <PatientDto>
        if (patient == null)
        {
            return Result<PatientDto>.Failure(new Error("Patient.NotFound", $"Patient with ID '{request.Id}' was not found."));
        }

        // 3. Map entity to DTO
        var patientDto = new PatientDto(
            patient.Id,
            patient.UserId,
            patient.FirstName,
            patient.LastName,
            patient.NationalId,
            patient.DateOfBirth,
            patient.Gender.ToString(),
            patient.BloodGroup.ToString(),
            patient.PhoneNumber,
            patient.Address,
            patient.EmergencyContactName,
            patient.EmergencyContactPhone
        );

        // 4. Return successful result explicitly typed with <PatientDto>
        return Result<PatientDto>.Success(patientDto);
    }
}