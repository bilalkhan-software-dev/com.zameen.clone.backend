using com.zameen.Models.Dto.Request;
using FluentValidation;

namespace com.zameen.Validator
{
    public class PropertyValidator : AbstractValidator<PropertyCreateRequest>
    {
        public PropertyValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);

            RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);

            RuleFor(x => x.Price).GreaterThan(0);

            RuleFor(x => x.City).NotEmpty().MaximumLength(100);

            RuleFor(x => x.Address).NotEmpty().MaximumLength(500);

            RuleFor(x => x.AreaSize).GreaterThan(0).WithMessage("Area Size must be greater than 0");

            RuleFor(x => x.AreaUnit).IsInEnum().WithMessage("Invalid area unit");

            RuleFor(x => x.PropertyType).IsInEnum().WithMessage("Invalid property type");

            RuleFor(x => x.Bedrooms).GreaterThanOrEqualTo(0);

            RuleFor(x => x.Bathrooms).GreaterThanOrEqualTo(0);
        }
    }
}
