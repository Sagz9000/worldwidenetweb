using System.Text;

namespace AssetPortal.Web.Helpers;

public static class ExportHelper
{
    public static string ToCsv(IEnumerable<IEnumerable<string>> rows)
    {
        var sb = new StringBuilder();

        foreach (var row in rows)
        {
            var cells = row.Select(cell =>
            {
                if (cell.IndexOfAny(new[] { ',', '"', '\n', '=' }) >= 0)
                {
                    return "\"" + cell.Replace("\"", "\"\"") + "\"";
                }
                return cell;
            });
            sb.AppendLine(string.Join(",", cells));
        }

        return sb.ToString();
    }
}
