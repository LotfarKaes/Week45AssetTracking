namespace Week45AssetTracking.Models;

/// <summary>
/// Level 5 — root object for JSON serialization (assets + next free ID).
/// </summary>
public class AssetCatalogFile
{
    public int NextId { get; set; } = 1;

    public List<AssetRecord> Assets { get; set; } = new();
}

/// <summary>
/// One asset stored on disk. <see cref="AssetKind"/> tells us which class to rebuild.
/// </summary>
public class AssetRecord
{
    public string AssetKind { get; set; } = string.Empty;

    public int Id { get; set; }

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public DateTime PurchaseDate { get; set; }

    public decimal PriceUsd { get; set; }

    public decimal PriceLocal { get; set; }

    public OfficeLocation Office { get; set; }

    /// <summary>Computers only: true = laptop, false = desktop.</summary>
    public bool? IsLaptop { get; set; }
}