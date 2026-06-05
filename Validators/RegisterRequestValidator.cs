using com.zameen.Models.Dto.Request;
using FluentValidation;

namespace com.zameen.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Full name is required and must not exceed 100 characters.");

        // If registering as an agency, AgencyName becomes required
        When(
            x => x.IsAgency,
            () =>
            {
                RuleFor(x => x.AgencyName)
                    .NotEmpty()
                    .MaximumLength(200)
                    .WithMessage("Agency name is required when registering as an agency.");

                RuleFor(x => x.ContactNumber)
                    .NotEmpty()
                    .WithMessage("Contact Number is required when registering as an agency.");

                RuleFor(x => x.Bio)
                    .NotEmpty()
                    .MaximumLength(200)
                    .WithMessage("Bio is required when registering as an agency.");

                RuleFor(x => x.ContactEmail)
                    .NotEmpty()
                    .WithMessage("Contact Email is required when registering as an agency.")
                    .EmailAddress()
                    .WithMessage("Invalid email address");
            }
        );
    }
}
