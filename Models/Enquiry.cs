

namespace com.zameen.Models
{
    public class Enquiry
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}