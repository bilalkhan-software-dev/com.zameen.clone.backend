using com.zameen.Models.Dto.Request;
using FluentValidation;

namespace com.zameen.Validators
{
    public class RevokeRequestValidator : AbstractValidator<RevokeRequest>
    {
        public RevokeRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh token is required to logout.");
        }
    }
}
