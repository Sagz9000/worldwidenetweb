using AssetPortal.Web.Helpers;
using AssetPortal.Web.Models;

namespace AssetPortal.Web.Data;

public static class DbSeeder
{
    public static void EnsureCreated(AppDbContext db)
    {
        db.Database.EnsureCreated();

        if (db.Users.Any())
        {
            return;
        }

        var admin = new User
        {
            Username = "admin",
            FullName = "System Administrator",
            Email = "admin@assetportal.local",
            PasswordHash = PasswordHasher.Hash("admin1234"),
            Department = "IT Operations",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow.AddDays(-400)
        };

        var support = new User
        {
            Username = "jsmith",
            FullName = "Jamie Smith",
            Email = "jsmith@assetportal.local",
            PasswordHash = PasswordHasher.Hash("Welcome1!"),
            Department = "Support Desk",
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow.AddDays(-300)
        };

        var finance = new User
        {
            Username = "pwang",
            FullName = "Priya Wang",
            Email = "pwang@assetportal.local",
            PasswordHash = PasswordHasher.Hash("Summer2024!"),
            Department = "Finance",
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow.AddDays(-60)
        };

        db.Users.AddRange(admin, support, finance);
        db.SaveChanges();

        var assets = new[]
        {
            new Asset { SerialNumber = "SN-XPS-1001", AssetType = "Laptop", Model = "Dell XPS 15", Location = "HQ - Floor 2", Status = AssetStatus.Assigned, OwnerId = support.Id, LastAudited = DateTime.UtcNow.AddDays(-30) },
            new Asset { SerialNumber = "SN-MBP-2044", AssetType = "Laptop", Model = "MacBook Pro 16", Location = "HQ - Floor 3", Status = AssetStatus.Assigned, OwnerId = finance.Id, LastAudited = DateTime.UtcNow.AddDays(-45) },
            new Asset { SerialNumber = "SN-SRV-500", AssetType = "Server", Model = "Dell PowerEdge R740", Location = "DC1", Status = AssetStatus.Assigned, OwnerId = admin.Id, LastAudited = DateTime.UtcNow.AddDays(-2) },
            new Asset { SerialNumber = "SN-Printer-901", AssetType = "Peripheral", Model = "HP LaserJet M404", Location = "HQ - Floor 1", Status = AssetStatus.Provisioned, LastAudited = DateTime.UtcNow.AddDays(-90) },
            new Asset { SerialNumber = "SN-LAP-77", AssetType = "Laptop", Model = "Lenovo ThinkPad T14", Location = "Satellite Office", Status = AssetStatus.Retired, LastAudited = DateTime.UtcNow.AddDays(-200) },
            new Asset { SerialNumber = "SN-MON-301", AssetType = "Monitor", Model = "Dell U2723QE", Location = "HQ - Floor 2", Status = AssetStatus.Assigned, OwnerId = support.Id, LastAudited = DateTime.UtcNow.AddDays(-5) }
        };

        db.Assets.AddRange(assets);
        db.SaveChanges();

        var laptop = assets[0];
        var ticket = new Ticket
        {
            Title = "Reimage laptop for new hire",
            Description = "Provision a clean image on the XPS unit in HQ.",
            Priority = TicketPriority.Medium,
            Status = TicketStatus.Open,
            AssetId = laptop.Id,
            ReporterId = support.Id,
            AssigneeId = support.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        ticket.Comments.Add(new TicketComment
        {
            Body = "Waiting on manager approval before wiping.",
            AuthorId = support.Id,
            CreatedAt = DateTime.UtcNow.AddHours(-20)
        });

        db.Tickets.Add(ticket);
        db.SaveChanges();

        db.WebhookSubscriptions.Add(new WebhookSubscription
        {
            Name = "Asset pipeline",
            EndpointUrl = "http://10.0.0.15/api/receive-asset",
            Secret = "whsec_local",
            EventType = "asset.created",
            CreatedById = admin.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        });
        db.SaveChanges();
    }
}
