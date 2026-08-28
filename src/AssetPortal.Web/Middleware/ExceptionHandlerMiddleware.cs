using AssetPortal.Web.Helpers;

namespace AssetPortal.Web.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/html; charset=utf-8";
            var html = $"""
                <!DOCTYPE html>
                <html><head><title>Error</title></head><body>
                {ErrorFormatter.Describe(ex)}
                </body></html>
                """;
            await context.Response.WriteAsync(html);
        }
    }
}
