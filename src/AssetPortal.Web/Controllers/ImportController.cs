using AssetPortal.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AssetPortal.Web.Controllers;

public class ImportController : Controller
{
    private readonly ImportService _import;

    public ImportController(ImportService import)
    {
        _import = import;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Restore(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ViewBag.Message = "No snapshot file supplied.";
            return View("Index");
        }

        using var stream = file.OpenReadStream();
        var result = await _import.RestoreSnapshot(stream);
        ViewBag.Message = result;
        return View("Index");
    }

    [HttpPost]
    public IActionResult Diff(string snapshotJson, string incomingJson)
    {
        var current = Parse(snapshotJson);
        var incoming = Parse(incomingJson);

        ViewBag.Message = _import.DiffSnapshots(current, incoming);
        return View("Index");
    }

    private static JObject Parse(string json)
    {
        return string.IsNullOrWhiteSpace(json) ? new JObject() : JsonConvert.DeserializeObject<JObject>(json)!;
    }
}
