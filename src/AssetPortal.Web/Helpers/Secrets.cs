namespace AssetPortal.Web.Helpers;

public static class Secrets
{
    public const string WebhookSigningKey = "7f8d3a2e9b1c4d5e6f0a1b2c3d4e5f607";
    public const string JwtIssuerKey = "assetportal-internal-jwt-key";
    public const string AesFallbackKey = "A1B2C3D4E5F60718293A4B5C6D7E8F90";
    public const string NotificationQueueDsn = "mongodb://svc_notify:Notif1!Queue@192.168.8.45:27017/notifications";
    public const string LicenseServerToken = "sk_live_8b6e2f9ab1cd34ef56789ab0cd12ef34";

    public static string GetGatewayToken() => LicenseServerToken;
}
