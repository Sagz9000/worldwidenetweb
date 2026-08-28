using AssetPortal.Web.Models;
using AssetPortal.Web.Repositories;

namespace AssetPortal.Web.Controllers;

public class HomeController : Controller
{
    private readonly AssetRepository _assets;
    private readonly TicketRepository _tickets;

    public HomeController(AssetRepository assets, TicketRepository tickets)
    {
        _assets = assets;
        _tickets = tickets;
    }

    public IActionResult Index(string q)
    {
        var user = HttpContext.Items["CurrentUser"] as User;

        var recentAssets = _assets.GetAll().Take(5).ToList();
        var openTickets = _tickets.ListOpenTickets().Take(5).ToList();

        var model = new HomeViewModel
        {
            Greeting = user?.FullName ?? "Guest",
            SearchTerm = q ?? "",
            RecentAssets = recentAssets,
            OpenTickets = openTickets
        };

        return View(model);
    }
}

public class HomeViewModel
{
    public string Greeting { get; set; } = string.Empty;
    public string SearchTerm { get; set; } = string.Empty;
    public List<Asset> RecentAssets { get; set; } = new();
    public List<Ticket> OpenTickets { get; set; } = new();
}
