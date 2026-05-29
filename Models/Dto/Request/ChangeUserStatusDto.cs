using com.zameen.Models.Enums;

namespace com.zameen.Models.Dto.Request;

public class ChangeUserStatusDto
{
    public string UserId { get; set; } = string.Empty;
    public AccountStatus NewStatus { get; set; }
}
