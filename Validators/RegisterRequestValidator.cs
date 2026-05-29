// Validators/RegisterRequestValidator.cs
using com.zameen.Models.Dto.Request;
using FluentValidation;

namespace com.zameen.Validators
{
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
                }
            );

            // Bio is optional, but if provided, limit length
            RuleFor(x => x.Bio)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Bio))
                .WithMessage("Bio must not exceed 500 characters.");
        }
    }
}
