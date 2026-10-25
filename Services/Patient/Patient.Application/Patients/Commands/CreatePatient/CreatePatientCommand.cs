using Healthcare.SharedKernel.Common;
using MediatR;
using Patient.Application.Common.Interfaces;
using Patient.Application.Patients.Dtos;
using Patient.Domain.Entities;
using Patient.Domain.Enums;

namespace Patient.Application.Patients.Commands.CreatePatient;

// Command to create a new patient profile (UserId is retrieved from JWT token)
public record CreatePatientCommand(
    string FirstName,
    string LastName,
    string NationalId,
    DateTime DateOfBirth,
    Gender Gender,
    BloodGroup BloodGroup,
    string PhoneNumber,
    string Address,
    string? EmergencyContactName = null,
    string? EmergencyContactPhone = null
) : IRequest<Result<PatientDto>>;

public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    private readonly IPatientDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public CreatePatientCommandHandler(IPatientDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PatientDto>> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        // 1. Retrieve current authenticated UserId from token
        var userId = _currentUserService.UserId;

        if (userId == Guid.Empty)
        {
            return Result<PatientDto>.Failure(new Error("User.Unauthenticated", "User identity could not be verified."));
        }

        // 2. Instantiate domain entity with verified userId
        var patient = new PatientEntity(
            userId,
            request.FirstName,
            request.LastName,
            request.NationalId,
            request.DateOfBirth,
            request.Gender,
            request.BloodGroup,
            request.PhoneNumber,
            request.Address,
            request.EmergencyContactName,
            request.EmergencyContactPhone
        );

        // 3. Persist entity in database
        _dbContext.Patients.Add(patient);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 4. Map created entity to DTO
        var dto = new PatientDto(
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

        return Result<PatientDto>.Success(dto);
    }
}