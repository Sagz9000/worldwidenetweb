using AssetPortal.Web.Helpers;

namespace AssetPortal.Web.Services;

public class WebhookService
{
    private static readonly HttpClient Http = new();

    public async Task<string> Deliver(string endpointUrl, string eventType, object payload)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(new
        {
            @event = eventType,
            sentAt = DateTimeOffset.UtcNow,
            data = payload
        });

        using var request = new HttpRequestMessage(HttpMethod.Post, endpointUrl)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
        request.Headers.Add("X-Webhook-Secret", Secrets.WebhookSigningKey);

        var response = await Http.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();
        return $"delivered to {endpointUrl} with status {(int)response.StatusCode}: {body}";
    }

    public async Task<byte[]> FetchExternalResource(string url, int maxBytes)
    {
        var response = await Http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        using var ms = new MemoryStream();
        await response.Content.CopyToAsync(ms);
        var data = ms.ToArray();
        if (data.Length > maxBytes)
        {
            return data[..maxBytes];
        }
        return data;
    }
}
