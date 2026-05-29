using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.zameen.Models.Dto.Response;

public class UserProfileResponse
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string UserName { get; set; } = string.Empty;
}
