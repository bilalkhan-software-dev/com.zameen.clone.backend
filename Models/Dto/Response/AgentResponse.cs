using com.zameen.Models.Enums;

namespace com.zameen.Models.Dto.Response;

public class AgentResponse : AbstractResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AgencyName { get; set; } = string.Empty;
    public string? ProfilePic { get; set; }
    public string? ContactNumber { get; set; } = string.Empty;
    public string? ContactEmail { get; set; } = string.Empty;
    public AccountStatus? AccountStatus { get; set; }
    public string? Bio { get; set; }
}
