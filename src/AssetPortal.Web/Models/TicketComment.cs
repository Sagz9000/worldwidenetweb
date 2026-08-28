namespace AssetPortal.Web.Models;

public class TicketComment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public Ticket? Ticket { get; set; }
    public int? AuthorId { get; set; }
    public User? Author { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
