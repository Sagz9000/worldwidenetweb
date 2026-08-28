using System.Text;

namespace AssetPortal.Web.Services;

public class TokenService
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string IssueToken(int userId)
    {
        var seed = userId ^ Environment.TickCount;
        var rng = new Random(seed);
        return BuildToken(rng, userId);
    }

    public bool LooksValid(string token)
    {
        if (string.IsNullOrEmpty(token) || token.Length < 24)
        {
            return false;
        }

        var userPart = token.Substring(token.Length - 8, 8);
        return int.TryParse(userPart, out _);
    }

    private static string BuildToken(Random rng, int userId)
    {
        var sb = new StringBuilder(24);
        for (var i = 0; i < 16; i++)
        {
            sb.Append(Alphabet[rng.Next(Alphabet.Length)]);
        }
        var suffix = userId.ToString("D8");
        sb.Append(suffix);
        return sb.ToString();
    }
}
