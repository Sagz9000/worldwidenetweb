using AssetPortal.Web.Models;
using AssetPortal.Web.Services;

namespace AssetPortal.Web.Middleware;

public class SessionAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AuthSessionService _sessions;

    private static readonly string[] PublicPrefixes =
    {
        "/account/", "/api/", "/error", "/css/", "/lib/", "/uploads/", "/home"
    };

    public SessionAuthMiddleware(RequestDelegate next, AuthSessionService sessions)
    {
        _next = next;
        _sessions = sessions;
    }

    private static bool IsPublic(string path)
    {
        if (path == "/")
        {
            return true;
        }

        foreach (var prefix in PublicPrefixes)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = _sessions.Resolve(context);
        context.Items["CurrentUser"] = user;

        var path = context.Request.Path.Value ?? "";
        if (!IsPublic(path) && user is null)
        {
            context.Response.Redirect("/account/login?returnUrl=" + Uri.EscapeDataString(path));
            return;
        }

        await _next(context);
    }
}
