namespace com.zameen.Models.Dto.Response;

public class EnquiryResponse : AbstractResponse
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Cnic { get; set; }
    public string? City { get; set; }
    public string? MonthlySalary { get; set; }
    public string EnquiryType { get; set; } = string.Empty;
}
