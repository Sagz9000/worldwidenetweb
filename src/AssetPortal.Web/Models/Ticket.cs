namespace AssetPortal.Web.Models;

public enum TicketPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Urgent = 3
}

public enum TicketStatus
{
    Open = 0,
    InProgress = 1,
    Resolved = 2,
    Closed = 3
}

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public int? AssetId { get; set; }
    public Asset? Asset { get; set; }
    public int? ReporterId { get; set; }
    public User? Reporter { get; set; }
    public int? AssigneeId { get; set; }
    public User? Assignee { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public List<TicketComment> Comments { get; set; } = new();
}
