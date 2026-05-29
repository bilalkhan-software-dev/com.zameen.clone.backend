using com.zameen.Models.Dto.Request;
using FluentValidation;

namespace com.zameen.Validators
{
    public class GoogleLoginRequestValidator : AbstractValidator<GoogleLoginRequest>
    {
        public GoogleLoginRequestValidator()
        {
            RuleFor(x => x.IdToken).NotEmpty().WithMessage("Google ID token is required.");
        }
    }
}
