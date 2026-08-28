namespace AssetPortal.Web.Models;

public enum AssetStatus
{
    Provisioned = 0,
    Assigned = 1,
    Retired = 2
}

public class Asset
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string AssetType { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public int? OwnerId { get; set; }
    public User? Owner { get; set; }
    public DateTime LastAudited { get; set; }
}
