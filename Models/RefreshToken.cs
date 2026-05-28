namespace com.zameen.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByIp { get; set; }
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }

        public Guid UserId { get; set; }
        public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
    }
}
