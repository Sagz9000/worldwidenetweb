namespace AssetPortal.Web.Models;

public class WebhookSubscription
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EndpointUrl { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
}
