namespace com.zameen.Models.Dto.Response;

public class EnquiryResponse
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Message { get; set; } = string.Empty;
}
