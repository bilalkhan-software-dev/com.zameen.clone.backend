using com.zameen.Models.Dto.Request;
using FluentValidation;

namespace com.zameen.Validators;

public class CreateEnquiryRequestValidator : AbstractValidator<CreateEnquiryRequest>
{
    public CreateEnquiryRequestValidator()
    {
        RuleFor(x => x.PropertyId).GreaterThan(0);
        RuleFor(x => x.SenderName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SenderEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Message).NotEmpty().MaximumLength(1000);
    }
}
