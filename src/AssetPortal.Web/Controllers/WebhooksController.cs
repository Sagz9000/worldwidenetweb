using AssetPortal.Web.Models;
using AssetPortal.Web.Services;

namespace AssetPortal.Web.Controllers;

public class WebhooksController : Controller
{
    private readonly AppDbContext _db;
    private readonly WebhookService _webhooks;

    public WebhooksController(AppDbContext db, WebhookService webhooks)
    {
        _db = db;
        _webhooks = webhooks;
    }

    public IActionResult Index()
    {
        return View(_db.WebhookSubscriptions.OrderBy(w => w.CreatedAt).ToList());
    }

    [HttpPost]
    public IActionResult Create(string name, string endpointUrl, string eventType)
    {
        var currentUser = HttpContext.Items["CurrentUser"] as User;

        _db.WebhookSubscriptions.Add(new WebhookSubscription
        {
            Name = name ?? "Untitled",
            EndpointUrl = endpointUrl ?? "",
            EventType = eventType ?? "asset.created",
            Secret = Guid.NewGuid().ToString("N"),
            CreatedById = currentUser?.Id ?? 0,
            CreatedAt = DateTime.UtcNow
        });
        _db.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Toggle(int id, bool enabled)
    {
        var hook = _db.WebhookSubscriptions.FirstOrDefault(w => w.Id == id);
        if (hook is not null)
        {
            hook.IsEnabled = enabled;
            _db.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Test(int id)
    {
        var hook = _db.WebhookSubscriptions.FirstOrDefault(w => w.Id == id);
        if (hook is null)
        {
            return NotFound();
        }

        var result = await _webhooks.Deliver(hook.EndpointUrl, "test.event", new { source = "portal", id });
        ViewBag.Result = result;
        return View("Index", _db.WebhookSubscriptions.OrderBy(w => w.CreatedAt).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> Fetch(string url)
    {
        var data = await _webhooks.FetchExternalResource(url ?? "", 4096);
        ViewBag.FetchResult = System.Text.Encoding.UTF8.GetString(data);
        return View("Index", _db.WebhookSubscriptions.OrderBy(w => w.CreatedAt).ToList());
    }
}
