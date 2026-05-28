using com.zameen.Models.Enums;

namespace com.zameen.Models.Dto.Response
{
    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public AccountStatus AccountStatus { get; set; }
        public IList<string>? Roles { get; set; } = [];
    }
}
