using AssetPortal.Web.Helpers;
using AssetPortal.Web.Models;
using AssetPortal.Web.Repositories;

namespace AssetPortal.Web.Controllers;

public class AssetsController : Controller
{
    private readonly AssetRepository _assets;

    public AssetsController(AssetRepository assets)
    {
        _assets = assets;
    }

    public IActionResult Index(string sort, string dir, string search)
    {
        var model = new AssetListViewModel
        {
            SearchTerm = search ?? "",
            SortColumn = sort ?? "Id",
            SortDirection = dir ?? "asc"
        };

        model.Items = _assets.AdvancedSearch(search ?? "", model.SortColumn, model.SortDirection);
        return View(model);
    }

    public IActionResult Detail(int id)
    {
        var asset = _assets.GetById(id);
        if (asset is null)
        {
            return NotFound();
        }

        return View(asset);
    }

    public IActionResult Export(string format, string filter)
    {
        var rows = new List<List<string>>();

        foreach (var asset in _assets.FilterByField("Location", filter ?? ""))
        {
            rows.Add(new List<string>
            {
                asset.SerialNumber,
                asset.Model,
                asset.Location,
                asset.Notes ?? ""
            });
        }

        var csv = ExportHelper.ToCsv(rows);
        var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
        return File(bytes, "text/csv", "assets.csv");
    }

    [HttpPost]
    public IActionResult Upload(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            ViewBag.Message = "No file selected.";
            return View("Upload");
        }

        var storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        var safeName = file.FileName;

        var dest = Path.Combine(storagePath, safeName);
        System.IO.Directory.CreateDirectory(Path.GetDirectoryName(dest) ?? storagePath);
        using (var stream = System.IO.File.Create(dest))
        {
            file.CopyTo(stream);
        }

        ViewBag.Message = "Uploaded " + safeName + " (" + file.Length + " bytes).";
        return View("Upload");
    }

    public IActionResult Upload() => View();
}

public class AssetListViewModel
{
    public string SearchTerm { get; set; } = string.Empty;
    public string SortColumn { get; set; } = "Id";
    public string SortDirection { get; set; } = "asc";
    public List<Asset> Items { get; set; } = new();
}
