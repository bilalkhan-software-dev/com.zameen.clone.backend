using com.zameen.Models.Dto.Request;
using FluentValidation;

namespace com.zameen.Validators;

public class UpdateAgentRequestValidator : AbstractValidator<UpdateAgentRequest>
{
    public UpdateAgentRequestValidator()
    {
        RuleFor(x => x.AgencyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Bio).MaximumLength(500);
    }
}
