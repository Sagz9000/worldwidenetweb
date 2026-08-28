using AssetPortal.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetPortal.Web.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();
    public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
    public DbSet<SessionEntry> Sessions => Set<SessionEntry>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var connection = _configuration.GetConnectionString("Default");
        if (string.IsNullOrEmpty(connection))
        {
            connection = "Data Source=assetportal.db";
        }

        optionsBuilder.UseSqlite(connection, sqlite =>
        {
            sqlite.CommandTimeout(30);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Asset>()
            .HasOne(a => a.Owner)
            .WithMany()
            .HasForeignKey(a => a.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Asset)
            .WithMany()
            .HasForeignKey(t => t.AssetId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Reporter)
            .WithMany()
            .HasForeignKey(t => t.ReporterId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Assignee)
            .WithMany()
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Ticket>()
            .HasMany(t => t.Comments)
            .WithOne(c => c.Ticket)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TicketComment>()
            .HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<SessionEntry>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
