using Healthcare.SharedKernel.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Patient.Application.Common.Interfaces;
using Patient.Application.Patients.Dtos;

namespace Patient.Application.Patients.Queries.GetMyProfile;

// Query to get the patient profile associated with the active authenticated user
public record GetMyProfileQuery : IRequest<Result<PatientDto>>;

public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<PatientDto>>
{
    private readonly IPatientDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetMyProfileQueryHandler(IPatientDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PatientDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        // 1. Get current authenticated UserId from token
        var userId = _currentUserService.UserId;

        if (userId == Guid.Empty)
        {
            return Result<PatientDto>.Failure(new Error("User.Unauthenticated", "User identity could not be verified."));
        }

        // 2. Query patient record matching the active UserId
        var patient = await _dbContext.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (patient == null)
        {
            return Result<PatientDto>.Failure(new Error("Patient.NotFound", "Patient profile does not exist for the current user."));
        }

        // 3. Map entity to DTO
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