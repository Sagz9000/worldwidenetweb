using AssetPortal.Web.Models;
using AssetPortal.Web.Repositories;

namespace AssetPortal.Web.Controllers;

public class ProfileController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserRepository _users;

    public ProfileController(AppDbContext db, UserRepository users)
    {
        _db = db;
        _users = users;
    }

    public IActionResult Index(int? userId)
    {
        var current = HttpContext.Items["CurrentUser"] as User;

        if (userId.HasValue)
        {
            return View(_users.Get(userId.Value));
        }

        return View(current);
    }

    public IActionResult Directory()
    {
        return View(_db.Users.OrderBy(u => u.FullName).ToList());
    }
}
