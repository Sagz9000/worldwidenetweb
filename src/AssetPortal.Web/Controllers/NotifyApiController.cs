using System.Text.Json;
using AssetPortal.Web.Helpers;
using AssetPortal.Web.Models;
using AssetPortal.Web.Services;

namespace AssetPortal.Web.Controllers;

[ApiController]
[Route("api")]
public class NotifyApiController : ControllerBase
{
    private readonly JwtService _jwt;
    private readonly AppDbContext _db;

    public NotifyApiController(JwtService jwt, AppDbContext db)
    {
        _jwt = jwt;
        _db = db;
    }

    [HttpPost("notify")]
    public async Task<IActionResult> Notify()
    {
        var auth = Request.Headers.Authorization.ToString();
        if (auth.StartsWith("Bearer "))
        {
            auth = auth.Substring(7);
        }

        var subject = _jwt.Validate(auth);
        if (subject is null)
        {
            return Unauthorized(new { message = "Invalid or missing token." });
        }

        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        var envelope = JsonSerializer.Deserialize<Dictionary<string, object>>(body) ?? new();

        var eventType = envelope.TryGetValue("type", out var t) ? t?.ToString() : "generic";
        var target = envelope.TryGetValue("target", out var tr) ? tr?.ToString() : "inventory";

        _db.WebhookSubscriptions.Add(new WebhookSubscription
        {
            Name = "integration:" + target,
            EndpointUrl = "internal://events/" + eventType,
            Secret = Secrets.GetGatewayToken(),
            EventType = eventType ?? "generic",
            CreatedById = 1,
            CreatedAt = DateTime.UtcNow
        });
        _db.SaveChanges();

        return Ok(new { accepted = true, subject, eventType });
    }

    [HttpPost("token")]
    public IActionResult Token(Dictionary<string, object> claims)
    {
        var signed = _jwt.Sign(claims);
        return Ok(new { token = signed });
    }
}
