using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;

namespace Week45AssetTracking.Services;

/// <summary>
/// Level 1 — list. Level 2 — type sort + EOL. Level 3 — offices + currency conversion.
/// </summary>
public class AssetManager
{
    private readonly List<Asset> _assets = new();
    private readonly CurrencyService _currencyService;

    public AssetManager(CurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    public CurrencyService CurrencyService => _currencyService;

    public IReadOnlyList<Asset> Assets => _assets;

    public bool HasAssets => _assets.Count > 0;

    public int Count => _assets.Count;

    public void AddAsset(Asset asset) => _assets.Add(asset);

    /// <summary>Level 3 — user enters USD; local price is calculated for the office currency.</summary>
    public Computer AddComputer(
        string brand,
        string model,
        DateTime purchaseDate,
        decimal priceUsd,
        OfficeLocation office,
        bool isLaptop)
    {
        decimal priceLocal = _currencyService.ConvertFromUsd(priceUsd, office);
        Computer computer = new(brand, model, purchaseDate, priceUsd, priceLocal, office, isLaptop);
        _assets.Add(computer);
        return computer;
    }

    public MobilePhone AddMobilePhone(
        string brand,
        string model,
        DateTime purchaseDate,
        decimal priceUsd,
        OfficeLocation office)
    {
        decimal priceLocal = _currencyService.ConvertFromUsd(priceUsd, office);
        MobilePhone phone = new(brand, model, purchaseDate, priceUsd, priceLocal, office);
        _assets.Add(phone);
        return phone;
    }

    /// <summary>Re-run conversion after refreshing live rates (Level 3).</summary>
    public void RecalculateAllLocalPrices()
    {
        foreach (Asset asset in _assets)
        {
            asset.PriceLocal = _currencyService.ConvertFromUsd(asset.PriceUsd, asset.Office);
        }
    }

    /// <summary>Level 2 — sort by asset type, then purchase date.</summary>
    public IEnumerable<Asset> GetSortedByTypeThenDate() =>
        _assets.OrderBy(a => a.GetAssetTypeKind()).ThenBy(a => a.PurchaseDate);

    /// <summary>Level 3 — sort by office name, then purchase date.</summary>
    public IEnumerable<Asset> GetSortedByOfficeThenDate() =>
        _assets.OrderBy(a => a.OfficeDisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(a => a.PurchaseDate);

    public void SeedSampleData()
    {
        _assets.Clear();

        AddComputer("Apple", "MacBook Pro", new DateTime(2024, 3, 15), 1500m, OfficeLocation.Sweden, true);
        AddComputer("Lenovo", "ThinkPad", new DateTime(2023, 1, 10), 1200m, OfficeLocation.Usa, true);
        AddComputer("Asus", "ZenBook", new DateTime(2022, 11, 5), 900m, OfficeLocation.Turkey, true);
        AddMobilePhone("Apple", "iPhone", new DateTime(2023, 6, 1), 999m, OfficeLocation.Sweden);
        AddMobilePhone("Samsung", "Galaxy", new DateTime(2023, 8, 20), 850m, OfficeLocation.Usa);
        AddMobilePhone("Nokia", "G42", new DateTime(2022, 12, 1), 299m, OfficeLocation.Turkey);

        DateTime redDemoPurchase = DateTime.Today.AddYears(-Asset.LifespanYears).AddMonths(4);
        DateTime yellowDemoPurchase = DateTime.Today.AddYears(-Asset.LifespanYears).AddMonths(2);

        AddComputer("Dell", "OptiPlex", redDemoPurchase, 700m, OfficeLocation.Usa, isLaptop: false);
        AddMobilePhone("Google", "Pixel", yellowDemoPurchase, 600m, OfficeLocation.Sweden);

        ConsoleUiHelper.WriteSuccess(
            "Sample assets loaded — prices converted to SEK / USD / TRY using current rates.");
    }

    public void ShowSortedWithEndOfLifeHighlights(IEnumerable<Asset> sorted, string heading)
    {
        List<Asset> list = sorted.ToList();

        Console.WriteLine();
        ConsoleUiHelper.WriteTitle(heading);
        ConsoleUiHelper.WriteMuted("Legend: YELLOW = under 3 months left | RED = under 6 months left");
        ConsoleUiHelper.WriteSeparator();

        if (list.Count == 0)
        {
            ConsoleUiHelper.WriteMuted("No assets to display.");
            return;
        }

        int index = 1;
        foreach (Asset asset in list)
        {
            ConsoleUiHelper.WriteAssetLine(asset, index);
            index++;
        }

        ConsoleUiHelper.WriteSeparator();
        WriteEndOfLifeSummary(list);
    }

    private static void WriteEndOfLifeSummary(List<Asset> list)
    {
        int yellow = list.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.YellowWarning);
        int red = list.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.RedWarning);
        int expired = list.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.Expired);

        ConsoleUiHelper.WriteSuccess($"Total assets: {list.Count}");
        if (yellow + red + expired > 0)
        {
            ConsoleUiHelper.WriteWarning($"Alerts — Yellow: {yellow}, Red: {red}, Expired: {expired}");
        }
    }
}