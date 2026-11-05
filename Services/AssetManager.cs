using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;

namespace Week45AssetTracking.Services;

/// <summary>
/// Level 1 — stores every asset in a <see cref="List{Asset}"/> and prints a friendly summary.
/// </summary>
public class AssetManager
{
    private readonly List<Asset> _assets = new();

    /// <summary>Read-only view of all assets in the order they were added.</summary>
    public IReadOnlyList<Asset> Assets => _assets;

    public bool HasAssets => _assets.Count > 0;

    public int Count => _assets.Count;

    /// <summary>Adds any asset (computer or phone) to the in-memory list.</summary>
    public void AddAsset(Asset asset)
    {
        _assets.Add(asset);
    }

    /// <summary>Prints every asset with a clean header and total count.</summary>
    public void ShowAllAssets()
    {
        Console.WriteLine();
        ConsoleUiHelper.WriteTitle("Company asset list");
        ConsoleUiHelper.WriteSeparator();

        if (!HasAssets)
        {
            ConsoleUiHelper.WriteMuted("No assets registered yet.");
            return;
        }

        // Added header row
        ConsoleUiHelper.WriteInfo("  #   Brand    Model      Purchase Date    Price (Local)    Office");
        ConsoleUiHelper.WriteSeparator();

        int index = 1;
        foreach (Asset asset in _assets)
        {
            ConsoleUiHelper.WriteInfo($"  {index}. {asset.ToDisplayLine()}");
            index++;
        }

        ConsoleUiHelper.WriteSeparator();
        ConsoleUiHelper.WriteSuccess($"Total assets: {Count}");
    }
}