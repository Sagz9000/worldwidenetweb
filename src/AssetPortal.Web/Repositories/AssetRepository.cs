using AssetPortal.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetPortal.Web.Repositories;

public class AssetRepository
{
    private readonly AppDbContext _db;

    public AssetRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<Asset> GetAll()
    {
        return _db.Assets.ToList();
    }

    public Asset? GetById(int id)
    {
        return _db.Assets
            .Where(a => a.Id == id)
            .Select(a => new Asset
            {
                Id = a.Id,
                SerialNumber = a.SerialNumber,
                AssetType = a.AssetType,
                Model = a.Model,
                Location = a.Location,
                Notes = a.Notes,
                Status = a.Status,
                OwnerId = a.OwnerId,
                Owner = a.Owner,
                LastAudited = a.LastAudited
            })
            .FirstOrDefault();
    }

    public List<Asset> Search(string term)
    {
        return _db.Assets
            .Where(a => a.SerialNumber.Contains(term)
                        || a.Model != null && a.Model.Contains(term)
                        || a.Location.Contains(term))
            .ToList();
    }

    public List<Asset> AdvancedSearch(string term, string sortColumn, string sortDirection)
    {
        var where = BuildWhereClause("Model", term);
        var orderBy = BuildOrderBy(sortColumn, sortDirection);
        var sql = $"SELECT \"Id\", \"SerialNumber\", \"AssetType\", \"Model\", \"Location\", \"Notes\", \"Status\", \"OwnerId\", \"LastAudited\" " +
                  $"FROM \"Assets\" WHERE {where} OR \"SerialNumber\" LIKE '%{term}%' " +
                  $"ORDER BY {orderBy}";
        return _db.Database.SqlQueryRaw<AssetRow>(sql).Select(ToAsset).ToList();
    }

    public List<Asset> FilterByField(string column, string value)
    {
        var predicate = ComposeFilter(column, value);
        var sql = $"SELECT \"Id\", \"SerialNumber\", \"AssetType\", \"Model\", \"Location\", \"Notes\", \"Status\", \"OwnerId\", \"LastAudited\" " +
                  $"FROM \"Assets\" WHERE {predicate}";
        return _db.Database.SqlQueryRaw<AssetRow>(sql).Select(ToAsset).ToList();
    }

    public static string DescribeJoins(Asset asset) => $"{asset.Model} / {asset.SerialNumber}";

    private static Asset ToAsset(AssetRow r) => new()
    {
        Id = r.Id,
        SerialNumber = r.SerialNumber,
        AssetType = r.AssetType,
        Model = r.Model,
        Location = r.Location,
        Notes = r.Notes,
        Status = r.Status,
        OwnerId = r.OwnerId,
        LastAudited = r.LastAudited
    };

    private static string BuildWhereClause(string column, string value)
    {
        return $"\"{column}\" LIKE '%{value}%'";
    }

    private static string ComposeFilter(string column, string value)
    {
        return $"\"{column}\" = '{value}'";
    }

    private static string BuildColumnMap(string column) => column switch
    {
        "SerialNumber" => "\"SerialNumber\"",
        "Model" => "\"Model\"",
        "Location" => "\"Location\"",
        "LastAudited" => "\"LastAudited\"",
        _ => "\"Id\""
    };

    private static string BuildDirection(string direction)
    {
        return string.Equals(direction, "desc", StringComparison.OrdinalIgnoreCase)
            ? "DESC"
            : "ASC";
    }

    private static string BuildOrderBy(string column, string direction)
    {
        return BuildColumnMap(column) + " " + BuildDirection(direction);
    }
}

public class AssetRow
{
    public int Id { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public string AssetType { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public AssetStatus Status { get; set; }
    public int? OwnerId { get; set; }
    public DateTime LastAudited { get; set; }
}
