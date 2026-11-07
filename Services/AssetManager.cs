using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;

namespace Week45AssetTracking.Services;

/// <summary>
/// Level 1 — list storage. Level 2 — LINQ sorting and end-of-life display.
/// </summary>
public class AssetManager
{
    private readonly List<Asset> _assets = new();

    public IReadOnlyList<Asset> Assets => _assets;

    public bool HasAssets => _assets.Count > 0;

    public int Count => _assets.Count;

    public void AddAsset(Asset asset) => _assets.Add(asset);

    /// <summary>
    /// Level 2 — sort by asset type, then by purchase date (oldest first within each type).
    /// </summary>
    public IEnumerable<Asset> GetSortedByTypeThenDate() =>
        _assets.OrderBy(a => a.GetAssetTypeKind()).ThenBy(a => a.PurchaseDate);

    /// <summary>
    /// Sample rows from the PDF plus two dates that trigger RED and YELLOW alerts today.
    /// </summary>
    public void SeedSampleData()
    {
        _assets.Clear();

        AddAsset(new Computer("Apple", "MacBook Pro", new DateTime(2024, 3, 15), 1500m, 15000m, "Sweden", true));
        AddAsset(new Computer("Lenovo", "ThinkPad", new DateTime(2023, 1, 10), 1200m, 1200m, "USA", true));
        AddAsset(new Computer("Asus", "ZenBook", new DateTime(2022, 11, 5), 900m, 30600m, "Turkey", true));
        AddAsset(new MobilePhone("Apple", "iPhone", new DateTime(2023, 6, 1), 999m, 10489m, "Sweden"));
        AddAsset(new MobilePhone("Samsung", "Galaxy", new DateTime(2023, 8, 20), 850m, 850m, "USA"));
        AddAsset(new MobilePhone("Nokia", "G42", new DateTime(2022, 12, 1), 299m, 10166m, "Turkey"));

        // End-of-life demo: purchased so that 3-year mark is ~4 months away (RED) and ~2 months (YELLOW).
        DateTime redDemoPurchase = DateTime.Today.AddYears(-Asset.LifespanYears).AddMonths(4);
        DateTime yellowDemoPurchase = DateTime.Today.AddYears(-Asset.LifespanYears).AddMonths(2);

        AddAsset(new Computer("Dell", "OptiPlex", redDemoPurchase, 700m, 700m, "USA", isLaptop: false));
        AddAsset(new MobilePhone("Google", "Pixel", yellowDemoPurchase, 600m, 6300m, "Sweden"));

        ConsoleUiHelper.WriteSuccess("Sample assets loaded (includes RED and YELLOW end-of-life examples).");
    }

    /// <summary>Level 1 style — insertion order, no sorting.</summary>
    public void ShowAllAssets()
    {
        Console.WriteLine();
        ConsoleUiHelper.WriteTitle("Company asset list (added order)");
        ConsoleUiHelper.WriteSeparator();

        if (!HasAssets)
        {
            ConsoleUiHelper.WriteMuted("No assets registered yet.");
            return;
        }

        int index = 1;
        foreach (Asset asset in _assets)
        {
            ConsoleUiHelper.WriteInfo($"  {index}. {asset.ToDisplayLine()}");
            index++;
        }

        ConsoleUiHelper.WriteSeparator();
        ConsoleUiHelper.WriteSuccess($"Total assets: {Count}");
    }

    /// <summary>Level 2 — sorted list with colorized end-of-life warnings.</summary>
    public void ShowSortedWithEndOfLifeHighlights()
    {
        List<Asset> sorted = GetSortedByTypeThenDate().ToList();

        Console.WriteLine();
        ConsoleUiHelper.WriteTitle("Asset list — sorted by type, then purchase date");
        ConsoleUiHelper.WriteMuted("Legend: YELLOW = under 3 months left | RED = under 6 months left");
        ConsoleUiHelper.WriteSeparator();

        if (sorted.Count == 0)
        {
            ConsoleUiHelper.WriteMuted("No assets to display.");
            return;
        }

        int index = 1;
        foreach (Asset asset in sorted)
        {
            ConsoleUiHelper.WriteAssetLine(asset, index);
            index++;
        }

        ConsoleUiHelper.WriteSeparator();

        int yellow = sorted.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.YellowWarning);
        int red = sorted.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.RedWarning);
        int expired = sorted.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.Expired);

        ConsoleUiHelper.WriteSuccess($"Total assets: {sorted.Count}");
        if (yellow + red + expired > 0)
        {
            ConsoleUiHelper.WriteWarning(
                $"Alerts — Yellow: {yellow}, Red: {red}, Expired: {expired}");
        }
    }
}