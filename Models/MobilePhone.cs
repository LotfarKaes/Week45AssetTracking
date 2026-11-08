namespace Week45AssetTracking.Models;

/// <summary>
/// Mobile phone asset (Level 1).
/// Examples from the assignment: iPhone, Samsung Galaxy, Nokia.
/// </summary>
public class MobilePhone : Asset
{
    /// <summary>Creates a mobile phone asset with the standard company fields.</summary>
    public MobilePhone(
        int id,
        string brand,
        string model,
        DateTime purchaseDate,
        decimal priceUsd,
        decimal priceLocal,
        OfficeLocation office)
        : base(id, brand, model, purchaseDate, priceUsd, priceLocal, office)
    {
    }

    /// <inheritdoc />
    public override string AssetType => "Mobile Phone";

    /// <inheritdoc />
    public override AssetTypeKind GetAssetTypeKind() => AssetTypeKind.MobilePhone;
}