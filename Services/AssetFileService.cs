using System.Text.Json;
using System.Text.Json.Serialization;
using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;

namespace Week45AssetTracking.Services;

/// <summary>
/// Level 5 — read/write <see cref="AssetCatalogFile"/> as JSON and export text reports.
/// </summary>
public class AssetFileService
{
    public const string DefaultFileName = "assets.json";

    public const string DefaultReportFileName = "asset-report.txt";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public bool FileExists() => File.Exists(DefaultFileName);

    public AssetCatalogFile Load()
    {
        string json = File.ReadAllText(DefaultFileName);
        return JsonSerializer.Deserialize<AssetCatalogFile>(json, JsonOptions)
               ?? new AssetCatalogFile();
    }

    public void Save(AssetCatalogFile catalog)
    {
        string json = JsonSerializer.Serialize(catalog, JsonOptions);
        File.WriteAllText(DefaultFileName, json);
    }

    /// <summary>Plain-text report for managers (sorted list passed in from caller).</summary>
    public void ExportReport(IReadOnlyList<Asset> assets, string filePath = DefaultReportFileName)
    {
        List<string> lines = new()
        {
            "Company Asset Report",
            $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}",
            new string('-', 72)
        };

        foreach (Asset asset in assets)
        {
            lines.Add(asset.ToDisplayLine());
            EndOfLifeStatus status = asset.GetEndOfLifeStatus();
            if (status != EndOfLifeStatus.Normal)
            {
                lines.Add($"  End-of-life status: {status}");
            }
        }

        lines.Add(new string('-', 72));
        lines.Add($"Total assets: {assets.Count}");

        File.WriteAllLines(filePath, lines);
        ConsoleUiHelper.WriteSuccess($"Report exported to {filePath}");
    }
}