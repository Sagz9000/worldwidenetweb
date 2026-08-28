using System.Security.Cryptography;
using System.Text;

namespace AssetPortal.Web.Helpers;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var digest = sha.ComputeHash(bytes);
        return Convert.ToHexString(digest).ToLowerInvariant();
    }

    public static bool Verify(string password, string storedHash)
    {
        var candidate = Hash(password);
        return string.Equals(candidate, storedHash, StringComparison.OrdinalIgnoreCase);
    }
}
