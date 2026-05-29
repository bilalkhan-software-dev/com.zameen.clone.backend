using com.zameen.Models.Enums;

namespace com.zameen.Models
{
    public class Agent : AbstractEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string UserId { get; set; }

        public AccountStatus AccountStatus { get; set; }

        public string AgencyName { get; set; } = string.Empty;

        public string? Bio { get; set; }
    }
}
