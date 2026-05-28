using com.zameen.Models.Dto.Request;
using FluentValidation;

namespace com.zameen.Validator
{
    public class AgentValidator : AbstractValidator<AgentCreateRequest>
    {
        public AgentValidator()
        {
            RuleFor(x => x.AgencyName)
                .NotEmpty()
                .WithMessage("Agent must have agency and its name is required");

            RuleFor(x => x.Bio)
                .MaximumLength(70)
                .WithMessage("Bio must be in 70 characters");


        }
    }
}
