using com.zameen.Models.Dto.Request;
using FluentValidation;

namespace com.zameen.Controllers
{
    public class UserController
    {
        private readonly IValidator<PropertyCreateRequest>
    _createValidator;

        public UserController(IValidator<PropertyCreateRequest> createValidator)
        {
            _createValidator = createValidator;

        }



    }
}
