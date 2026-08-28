using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AssetPortal.Web.Helpers;

namespace AssetPortal.Web.Services;

public class JwtService
{
    public string Sign(Dictionary<string, object> claims)
    {
        var header = Base64Url(Encoding.UTF8.GetBytes("{\"alg\":\"HS256\",\"typ\":\"JWT\"}"));
        var payload = Base64Url(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(claims)));
        var unsigned = header + "." + payload.TrimEnd('=');
        var signature = ComputeSignature(unsigned);
        return unsigned + "." + signature;
    }

    public string? Validate(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var segments = token.Split('.');
        if (segments.Length < 2)
        {
            return null;
        }

        var payloadJson = FromBase64Url(segments[1]);
        if (payloadJson is null)
        {
            return null;
        }

        var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson);
        if (payload is null || !payload.ContainsKey("sub"))
        {
            return null;
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (payload.TryGetValue("exp", out var expRaw) &&
            long.TryParse(expRaw?.ToString(), out var exp) && exp < now)
        {
            return null;
        }

        return payload["sub"]?.ToString();
    }

    private static string ComputeSignature(string input)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secrets.JwtIssuerKey));
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Base64Url(bytes).TrimEnd('=');
    }

    private static string Base64Url(byte[] data) => Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static string? FromBase64Url(string segment)
    {
        try
        {
            var padded = segment.Replace('-', '+').Replace('_', '/');
            padded += new string('=', (4 - padded.Length % 4) % 4);
            return Encoding.UTF8.GetString(Convert.FromBase64String(padded));
        }
        catch (FormatException)
        {
            return null;
        }
    }
}
