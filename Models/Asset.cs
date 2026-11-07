namespace Week45AssetTracking.Models;

/// <summary>
/// Base class for every company asset (Level 1).
/// Level 2 adds lifespan, end-of-life checks, and sorting support.
/// </summary>
public abstract class Asset
{
    /// <summary>Business rule from the assignment: every asset lasts 3 years.</summary>
    public const int LifespanYears = 3;

    /// <summary>Creates an asset with the common fields every device shares.</summary>
    protected Asset(
        string brand,
        string model,
        DateTime purchaseDate,
        decimal priceUsd,
        decimal priceLocal,
        OfficeLocation office)
    {
        Brand = brand;
        Model = model;
        PurchaseDate = purchaseDate;
        PriceUsd = priceUsd;
        PriceLocal = priceLocal;
        Office = office;
    }

    /// <summary>Manufacturer name, e.g. Apple or Samsung.</summary>
    public string Brand { get; set; }

    /// <summary>Product name, e.g. MacBook Pro or Galaxy.</summary>
    public string Model { get; set; }

    /// <summary>When the company bought the device.</summary>
    public DateTime PurchaseDate { get; set; }

    /// <summary>Purchase price in US dollars.</summary>
    public decimal PriceUsd { get; set; }

    /// <summary>Same purchase expressed in the office local currency (entered or calculated later).</summary>
    public decimal PriceLocal { get; set; }

    /// <summary>Which company office owns this asset (Level 3).</summary>
    public OfficeLocation Office { get; set; }

    /// <summary>Readable office name, e.g. Sweden.</summary>
    public string OfficeDisplayName => OfficeInfo.GetDisplayName(Office);

    /// <summary>Local currency for this office, e.g. SEK.</summary>
    public string LocalCurrencyCode => OfficeInfo.GetCurrencyCode(Office);

    /// <summary>Human-readable type: Laptop, Desktop, Mobile Phone, etc.</summary>
    public abstract string AssetType { get; }

    /// <summary>Enum used for LINQ sorting by asset type (Level 2).</summary>
    public abstract AssetTypeKind GetAssetTypeKind();

    /// <summary>Last day the asset is considered in service (purchase date + 3 years).</summary>
    public DateTime EndOfLifeDate => PurchaseDate.AddYears(LifespanYears);

    /// <summary>Time left until <see cref="EndOfLifeDate"/> (can be negative if expired).</summary>
    public TimeSpan TimeUntilEndOfLife(DateTime? asOfDate = null)
    {
        DateTime today = (asOfDate ?? DateTime.Today).Date;
        return EndOfLifeDate.Date - today;
    }

    /// <summary>
    /// Applies assignment rules: yellow &lt; 3 months left, red &lt; 6 months left, else normal.
    /// </summary>
    public EndOfLifeStatus GetEndOfLifeStatus(DateTime? asOfDate = null)
    {
        TimeSpan remaining = TimeUntilEndOfLife(asOfDate);

        if (remaining <= TimeSpan.Zero)
        {
            return EndOfLifeStatus.Expired;
        }

        if (remaining < TimeSpan.FromDays(30 * 3))
        {
            return EndOfLifeStatus.YellowWarning;
        }

        if (remaining < TimeSpan.FromDays(30 * 6))
        {
            return EndOfLifeStatus.RedWarning;
        }

        return EndOfLifeStatus.Normal;
    }

    /// <summary>One formatted line for the console summary.</summary>
    public virtual string ToDisplayLine()
    {
        return
            $"{AssetType} | {Brand} {Model} | Office: {OfficeDisplayName} | " +
            $"Purchased: {PurchaseDate:yyyy-MM-dd} | End-of-life: {EndOfLifeDate:yyyy-MM-dd} | " +
            $"USD {PriceUsd:N2} | {LocalCurrencyCode} {PriceLocal:N2}";
    }

    /// <inheritdoc />
    public override string ToString() => ToDisplayLine();
}