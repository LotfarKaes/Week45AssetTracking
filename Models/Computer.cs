namespace Week45AssetTracking.Models;

/// <summary>
/// Laptop or desktop computer (Level 1).
/// Examples from the assignment: MacBook Pro, ThinkPad, ZenBook.
/// </summary>
public class Computer : Asset
{
    /// <summary>
    /// Creates a computer asset. Set <paramref name="isLaptop"/> to false for a desktop.
    /// </summary>
    public Computer(
        string brand,
        string model,
        DateTime purchaseDate,
        decimal priceUsd,
        decimal priceLocal,
        OfficeLocation office,
        bool isLaptop)
        : base(brand, model, purchaseDate, priceUsd, priceLocal, office)
    {
        IsLaptop = isLaptop;
    }

    /// <summary>True for laptops; false for desktop machines.</summary>
    public bool IsLaptop { get; }

    /// <inheritdoc />
    public override string AssetType => IsLaptop ? "Laptop" : "Desktop";

    /// <inheritdoc />
    public override AssetTypeKind GetAssetTypeKind() =>
        IsLaptop ? AssetTypeKind.Laptop : AssetTypeKind.Desktop;
}