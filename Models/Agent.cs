using com.zameen.Models.Enums;

namespace com.zameen.Models
{
    public class Agent : AbstractEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string AgencyName { get; set; } = string.Empty;

        public string? Bio { get; set; }
    }
}
