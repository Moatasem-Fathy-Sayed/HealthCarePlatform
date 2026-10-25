using Healthcare.SharedKernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Patient.Application.Common.Interfaces;
using Patient.Application.Patients.Dtos;
using Patient.Domain.Enums;

namespace Patient.Application.Patients.Commands.UpdatePatient;

// Command to update editable patient profile details
public record UpdatePatientCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Address,
    BloodGroup BloodGroup,
    string? EmergencyContactName = null,
    string? EmergencyContactPhone = null
) : IRequest<Result<PatientDto>>;

public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Result<PatientDto>>
{
    private readonly IPatientDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public UpdatePatientCommandHandler(IPatientDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PatientDto>> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        // 1. Get current authenticated UserId from JWT token
        var userId = _currentUserService.UserId;

        if (userId == Guid.Empty)
        {
            return Result<PatientDto>.Failure(new Error("User.Unauthenticated", "User identity could not be verified."));
        }

        // 2. Fetch patient profile matching active UserId
        var patient = await _dbContext.Patients
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (patient == null)
        {
            return Result<PatientDto>.Failure(new Error("Patient.NotFound", "Patient profile does not exist for the current user."));
        }

        // 3. Update domain entity using UpdateProfile method
        patient.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Address,
            request.BloodGroup,
            request.EmergencyContactName,
            request.EmergencyContactPhone
        );

        // 4. Save changes in database
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 5. Map updated entity to DTO
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