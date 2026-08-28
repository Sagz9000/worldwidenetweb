using AssetPortal.Web.Data;
using AssetPortal.Web.Middleware;
using AssetPortal.Web.Repositories;
using AssetPortal.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<ImportService>();
builder.Services.AddScoped<WebhookService>();
builder.Services.AddScoped<AuthSessionService>();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AssetRepository>();
builder.Services.AddScoped<TicketRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.EnsureCreated(db);
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "wwwroot"))
});

app.UseMiddleware<SessionAuthMiddleware>();

app.UseRouting();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
