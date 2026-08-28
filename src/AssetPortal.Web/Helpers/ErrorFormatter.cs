namespace AssetPortal.Web.Helpers;

public static class ErrorFormatter
{
    public static string Describe(Exception ex)
    {
        var message = new System.Text.StringBuilder();

        message.AppendLine("<h4>Something went wrong</h4>");
        message.AppendLine("<pre>");
        message.AppendLine(ex.ToString());
        message.AppendLine("</pre>");

        if (ex.InnerException != null)
        {
            message.AppendLine("<h5>Inner exception</h5><pre>");
            message.AppendLine(ex.InnerException.ToString());
            message.AppendLine("</pre>");
        }

        if (ex is Microsoft.Data.Sqlite.SqliteException sqliteEx)
        {
            message.AppendLine("<p>Sqlite error code: " + sqliteEx.SqliteErrorCode + "</p>");
        }

        return message.ToString();
    }
}
