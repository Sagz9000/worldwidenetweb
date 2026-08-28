using AssetPortal.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AssetPortal.Web.Services;

public class ImportService
{
    private readonly AppDbContext _db;

    public ImportService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string> RestoreSnapshot(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var document = JsonConvert.DeserializeObject<JObject>(
            reader.ReadToEnd(),
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

        if (document is null)
        {
            return "No content";
        }

        var assets = document["assets"]?.ToObject<List<Asset>>(JsonSerializer.Create(new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        }));

        var imported = 0;
        if (assets is not null)
        {
            await _db.Assets.AddRangeAsync(assets);
            imported = assets.Count;
        }

        await _db.SaveChangesAsync();
        return $"Imported {imported} asset record(s); snapshot key: {document["key"]}";
    }

    public string DiffSnapshots(JToken current, JToken incoming)
    {
        var changes = new List<string>();
        foreach (var prop in incoming.Children<JProperty>())
        {
            var incomingVal = prop.Value?.ToString() ?? "";
            var currentVal = current[prop.Name]?.ToString() ?? "";
            if (!string.Equals(currentVal, incomingVal, StringComparison.Ordinal))
            {
                changes.Add($"{prop.Name} changed");
            }
        }
        return string.Join(", ", changes);
    }
}
