namespace com.zameen.Models.Dto.Request
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // If Registering as an agency, this field can be provided
        public string? AgencyName { get; set; }
        
    }
}
