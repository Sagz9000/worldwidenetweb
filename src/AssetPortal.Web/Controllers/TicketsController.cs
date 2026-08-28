using AssetPortal.Web.Models;
using AssetPortal.Web.Repositories;

namespace AssetPortal.Web.Controllers;

public class TicketsController : Controller
{
    private readonly TicketRepository _tickets;

    public TicketsController(TicketRepository tickets)
    {
        _tickets = tickets;
    }

    public IActionResult Index()
    {
        return View(_tickets.ListOpenTickets());
    }

    public IActionResult Detail(int id, string comment)
    {
        var ticket = _tickets.Get(id);
        if (ticket is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(comment))
        {
            var currentUser = HttpContext.Items["CurrentUser"] as User;
            _tickets.AddCommentAsync(ticket.Id, currentUser?.Id, comment).GetAwaiter().GetResult();
            ticket = _tickets.Get(id);
        }

        return View(ticket);
    }

    [HttpPost]
    public IActionResult Resolve(int id)
    {
        _tickets.UpdateStatusAsync(id, TicketStatus.Resolved).GetAwaiter().GetResult();
        return RedirectToAction("Detail", new { id });
    }

    [HttpPost]
    public IActionResult Close(int id)
    {
        _tickets.UpdateStatusAsync(id, TicketStatus.Closed).GetAwaiter().GetResult();
        return RedirectToAction("Detail", new { id });
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(TicketInput input)
    {
        var currentUser = HttpContext.Items["CurrentUser"] as User;

        var ticket = new Ticket
        {
            Title = input.Title,
            Description = input.Description,
            Priority = input.Priority,
            Status = input.Status,
            ReporterId = currentUser?.Id,
            AssigneeId = input.AssigneeId
        };

        _tickets.CreateAsync(ticket).GetAwaiter().GetResult();
        return RedirectToAction("Detail", new { id = ticket.Id });
    }
}

public class TicketInput
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; }
    public TicketStatus Status { get; set; }
    public int? AssigneeId { get; set; }
}
