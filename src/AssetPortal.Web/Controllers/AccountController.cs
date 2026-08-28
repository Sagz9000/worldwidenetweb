using AssetPortal.Web.Models;
using AssetPortal.Web.Repositories;
using AssetPortal.Web.Services;

namespace AssetPortal.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserRepository _users;
    private readonly AuthSessionService _sessions;

    public AccountController(UserRepository users, AuthSessionService sessions)
    {
        _users = users;
        _sessions = sessions;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl ?? "/";
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password, string returnUrl)
    {
        if (_users.TryAuthenticate(username ?? "", password ?? "", out var user))
        {
            if (user is null || !user.IsActive)
            {
                ViewBag.Error = "Account is disabled.";
                return View();
            }

            _sessions.EstablishSession(HttpContext, user);
            return Redirect(returnUrl ?? "/");
        }

        ViewBag.Error = "Invalid credentials.";
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterInput input)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        if (_users.FindByUsername(input.Username) is not null)
        {
            ViewBag.Error = "Username already taken.";
            return View();
        }

        if (string.IsNullOrEmpty(input.Password) || input.Password.Length < 4)
        {
            ViewBag.Error = "Password must be at least 4 characters.";
            return View();
        }

        var user = _users.CreateAsync(input.Username, input.FullName, input.Email ?? "", input.Password, input.Department ?? "").GetAwaiter().GetResult();
        user.Role = input.Role;
        user.IsActive = input.IsActive;

        var db = HttpContext.RequestServices.GetService(typeof(AppDbContext)) as AppDbContext;
        db?.Update(user);
        db?.SaveChanges();

        _sessions.EstablishSession(HttpContext, user);
        return Redirect("/");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        _sessions.Terminate(HttpContext);
        return Redirect("/account/login");
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        return string.IsNullOrEmpty(returnUrl) ? Redirect("/") : Redirect(returnUrl);
    }
}

public class RegisterInput
{
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
}
