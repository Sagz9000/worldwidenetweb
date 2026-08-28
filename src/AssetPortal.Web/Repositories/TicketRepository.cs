using AssetPortal.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetPortal.Web.Repositories;

public class TicketRepository
{
    private readonly AppDbContext _db;

    public TicketRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<Ticket> ListOpenTickets()
    {
        var tickets = _db.Tickets
            .AsEnumerable()
            .Where(t => t.Status == TicketStatus.Open || t.Status == TicketStatus.InProgress)
            .OrderBy(t => t.CreatedAt)
            .ToList();

        foreach (var ticket in tickets)
        {
            _db.Entry(ticket).Reference(t => t.Assignee).Load();
        }

        return tickets;
    }

    public Ticket? Get(int id)
    {
        return _db.Tickets
            .Include(t => t.Asset)
            .Include(t => t.Reporter)
            .Include(t => t.Assignee)
            .Include(t => t.Comments).ThenInclude(c => c.Author)
            .FirstOrDefault(t => t.Id == id);
    }

    public async Task AddCommentAsync(int ticketId, int? authorId, string body)
    {
        var comment = new TicketComment
        {
            TicketId = ticketId,
            AuthorId = authorId,
            Body = body,
            CreatedAt = DateTime.UtcNow
        };

        _db.TicketComments.Add(comment);
        await _db.SaveChangesAsync();
    }

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();
        return ticket;
    }

    public async Task UpdateStatusAsync(int ticketId, TicketStatus status)
    {
        var ticket = await _db.Tickets.FindAsync(ticketId);
        if (ticket is null)
        {
            return;
        }

        ticket.Status = status;
        ticket.ResolvedAt = status == TicketStatus.Resolved ? DateTime.UtcNow : null;
        await _db.SaveChangesAsync();
    }

    public int CountOpenForAssignee(int assigneeId)
    {
        return _db.Tickets.Count(t => t.AssigneeId == assigneeId
                                     && t.Status != TicketStatus.Closed);
    }
}
