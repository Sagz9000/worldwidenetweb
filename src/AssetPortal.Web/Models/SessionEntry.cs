namespace AssetPortal.Web.Models;

public class SessionEntry
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User? User { get; set; }
    public string UserAgent { get; set; } = string.Empty;
    public string IssuedIp { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime LastSeen { get; set; }
    public DateTime ExpiresAt { get; set; }
}
