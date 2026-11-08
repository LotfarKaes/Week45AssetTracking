namespace Week45AssetTracking.Models;

/// <summary>
/// Abstract base for all hardware (Level 1). Level 4: implements <see cref="ICompanyAsset"/> and adds ID + age.
/// </summary>
public abstract class Asset : ICompanyAsset
{
    /// <summary>Business rule from the assignment: every asset lasts 3 years.</summary>
    public const int LifespanYears = 3;

    /// <summary>Creates an asset with the common fields every device shares.</summary>
    protected Asset(
        int id,
        string brand,
        string model,
        DateTime purchaseDate,
        decimal priceUsd,
        decimal priceLocal,
        OfficeLocation office)
    {
        Id = id;
        Brand = brand;
        Model = model;
        PurchaseDate = purchaseDate;
        PriceUsd = priceUsd;
        PriceLocal = priceLocal;
        Office = office;
    }

    /// <summary>Unique asset number assigned by <see cref="Services.AssetManager"/> (Level 4).</summary>
    public int Id { get; set; }

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

    /// <summary>Elapsed time since purchase (Level 4).</summary>
    public TimeSpan Age => DateTime.Today - PurchaseDate.Date;

    /// <summary>Friendly age text, e.g. "1 year, 3 months".</summary>
    public string AgeDescription => FormatAge(Age);

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
            $"#{Id} | {AssetType} | {Brand} {Model} | Office: {OfficeDisplayName} | " +
            $"Purchased: {PurchaseDate:yyyy-MM-dd} | Age: {AgeDescription} | " +
            $"End-of-life: {EndOfLifeDate:yyyy-MM-dd} | " +
            $"USD {PriceUsd:N2} | {LocalCurrencyCode} {PriceLocal:N2}";
    }

    /// <inheritdoc />
    public override string ToString() => ToDisplayLine();

    private static string FormatAge(TimeSpan age)
    {
        if (age.TotalDays < 0)
        {
            return "0 days";
        }

        int years = (int)(age.TotalDays / 365.25);
        int months = (int)((age.TotalDays % 365.25) / 30.44);

        if (years == 0 && months == 0)
        {
            int days = Math.Max(0, (int)age.TotalDays);
            return days == 1 ? "1 day" : $"{days} days";
        }

        List<string> parts = new();
        if (years > 0)
        {
            parts.Add(years == 1 ? "1 year" : $"{years} years");
        }

        if (months > 0)
        {
            parts.Add(months == 1 ? "1 month" : $"{months} months");
        }

        return string.Join(", ", parts);
    }
}