namespace Week45AssetTracking.Models;

/// <summary>
/// Base class for every company asset (Level 1).
/// Shared properties match the assignment table: brand, model, dates, prices, office, and type.
/// </summary>
public abstract class Asset
{
    /// <summary>Creates an asset with the common fields every device shares.</summary>
    protected Asset(
        string brand,
        string model,
        DateTime purchaseDate,
        decimal priceUsd,
        decimal priceLocal,
        string office)
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

    /// <summary>Office country or site, e.g. Sweden.</summary>
    public string Office { get; set; }

    /// <summary>Human-readable type: Laptop, Desktop, Mobile Phone, etc.</summary>
    public abstract string AssetType { get; }

    /// <summary>One formatted line for the console summary.</summary>
    public virtual string ToDisplayLine()
    {
        return
            $"{AssetType} | {Brand} {Model} | Office: {Office} | " +
            $"Purchased: {PurchaseDate:yyyy-MM-dd} | " +
            $"USD {PriceUsd:N2} | Local {PriceLocal:N2}";
    }

    /// <inheritdoc />
    public override string ToString() => ToDisplayLine();
}