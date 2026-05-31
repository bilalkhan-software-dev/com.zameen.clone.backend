namespace com.zameen.Models.Dto.Request;

public class CreateEnquiryRequest
{
    public int PropertyId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Message { get; set; } = string.Empty;
}
