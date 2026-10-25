using FluentValidation;

namespace Patient.Application.Patients.Commands.CreatePatient;

// Validator class inherits from AbstractValidator for CreatePatientCommand
public class CreatePatientCommandValidator : AbstractValidator<CreatePatientCommand>
{
    public CreatePatientCommandValidator()
    {
        // 1. Validate FirstName
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        // 2. Validate LastName
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        // 3. Validate NationalId (Egyptian National ID format: 14 digits)
        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("National ID is required.")
            .Matches(@"^\d{14}$").WithMessage("National ID must consist of exactly 14 digits.");

        // 4. Validate DateOfBirth (Patient must not be born in the future)
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past.");

        // 5. Validate Gender & BloodGroup enums
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value.");

        RuleFor(x => x.BloodGroup)
            .IsInEnum().WithMessage("Invalid blood group value.");

        // 6. Validate PhoneNumber (Egyptian mobile number format)
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^01[0125]\d{8}$").WithMessage("Phone number must be a valid Egyptian mobile number (e.g., 01012345678).");

        // 7. Validate Address
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");
    }
}