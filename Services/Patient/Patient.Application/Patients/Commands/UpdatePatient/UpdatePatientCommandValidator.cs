using FluentValidation;

namespace Patient.Application.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandValidator : AbstractValidator<UpdatePatientCommand>
{
    public UpdatePatientCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^01[0125]\d{8}$").WithMessage("Invalid Egyptian phone number format.");

        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

        RuleFor(x => x.BloodGroup)
            .IsInEnum().WithMessage("Invalid blood group selection.");
    }
}