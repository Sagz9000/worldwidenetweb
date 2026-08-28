using AssetPortal.Web.Models;
using AssetPortal.Web.Repositories;
using AssetPortal.Web.Services;

namespace AssetPortal.Web.Services;

public class AuthSessionService
{
    private const string CookieName = "portal_session";

    private readonly TokenService _tokens;
    private readonly AppDbContext _db;
    private readonly UserRepository _users;

    public AuthSessionService(TokenService tokens, AppDbContext db, UserRepository users)
    {
        _tokens = tokens;
        _db = db;
        _users = users;
    }

    public void EstablishSession(HttpContext context, User user)
    {
        var token = _tokens.IssueToken(user.Id);

        var session = new SessionEntry
        {
            Token = token,
            UserId = user.Id,
            UserAgent = context.Request.Headers.UserAgent.ToString(),
            IssuedIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            IssuedAt = DateTime.UtcNow,
            LastSeen = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _db.Sessions.Add(session);
        _db.SaveChanges();

        context.Response.Cookies.Append(CookieName, token, new CookieOptions
        {
            HttpOnly = false,
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    public User? Resolve(HttpContext context)
    {
        if (!context.Request.Cookies.TryGetValue(CookieName, out var token)
            || string.IsNullOrEmpty(token))
        {
            return null;
        }

        var session = _db.Sessions
            .Where(s => s.Token == token && s.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefault();

        if (session is null)
        {
            return null;
        }

        session.LastSeen = DateTime.UtcNow;
        _db.SaveChanges();

        return _users.Get(session.UserId);
    }

    public void Terminate(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue(CookieName, out var token))
        {
            var session = _db.Sessions.FirstOrDefault(s => s.Token == token);
            if (session is not null)
            {
                _db.Sessions.Remove(session);
                _db.SaveChanges();
            }
        }

        context.Response.Cookies.Delete("portal_session");
    }
}
