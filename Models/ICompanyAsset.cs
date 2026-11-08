namespace Week45AssetTracking.Models;

/// <summary>
/// Level 4 — interface for polymorphic handling of trackable company assets.
/// <see cref="Asset"/> implements this contract; lists can use <see cref="Asset"/> or this interface.
/// </summary>
public interface ICompanyAsset
{
    /// <summary>Unique identifier within the company register.</summary>
    int Id { get; }

    /// <summary>Short type label (Laptop, Mobile Phone, …).</summary>
    string AssetType { get; }

    /// <summary>How long the device has been in use, in plain language.</summary>
    string AgeDescription { get; }

    /// <summary>One-line summary for console output.</summary>
    string ToDisplayLine();
}