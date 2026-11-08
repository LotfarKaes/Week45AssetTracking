using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;

namespace Week45AssetTracking.Services;

/// <summary>
/// Central register: list storage, IDs, sorting, search, and statistics (Levels 1–4).
/// </summary>
public class AssetManager
{
    private readonly List<Asset> _assets = new();
    private readonly CurrencyService _currencyService;
    private int _nextId = 1;

    public AssetManager(CurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    public CurrencyService CurrencyService => _currencyService;

    public IReadOnlyList<Asset> Assets => _assets;

    public bool HasAssets => _assets.Count > 0;

    public int Count => _assets.Count;

    public int NextId => _nextId;

    public Asset? FindById(int id) => _assets.FirstOrDefault(a => a.Id == id);

    private int AllocateId() => _nextId++;

    public Computer AddComputer(
        string brand,
        string model,
        DateTime purchaseDate,
        decimal priceUsd,
        OfficeLocation office,
        bool isLaptop)
    {
        int id = AllocateId();
        decimal priceLocal = _currencyService.ConvertFromUsd(priceUsd, office);
        Computer computer = new(id, brand, model, purchaseDate, priceUsd, priceLocal, office, isLaptop);
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
        int id = AllocateId();
        decimal priceLocal = _currencyService.ConvertFromUsd(priceUsd, office);
        MobilePhone phone = new(id, brand, model, purchaseDate, priceUsd, priceLocal, office);
        _assets.Add(phone);
        return phone;
    }

    public bool RemoveById(int id)
    {
        Asset? asset = FindById(id);
        return asset is not null && _assets.Remove(asset);
    }

    public void RecalculateAllLocalPrices()
    {
        foreach (Asset asset in _assets)
        {
            asset.PriceLocal = _currencyService.ConvertFromUsd(asset.PriceUsd, asset.Office);
        }
    }

    public IEnumerable<Asset> GetSortedByTypeThenDate() =>
        _assets.OrderBy(a => a.GetAssetTypeKind()).ThenBy(a => a.PurchaseDate);

    public IEnumerable<Asset> GetSortedByOfficeThenDate() =>
        _assets.OrderBy(a => a.OfficeDisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(a => a.PurchaseDate);

    public IEnumerable<Asset> SearchByBrandOrModel(string term)
    {
        string needle = term.Trim();
        return _assets.Where(a =>
            a.Brand.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
            a.Model.Contains(needle, StringComparison.OrdinalIgnoreCase));
    }

    public void SeedSampleData()
    {
        _assets.Clear();
        _nextId = 1;

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

        ConsoleUiHelper.WriteSuccess("Sample assets loaded with IDs #1–#8.");
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

        foreach (Asset asset in list)
        {
            ConsoleUiHelper.WriteAssetLine(asset);
        }

        ConsoleUiHelper.WriteSeparator();
        WriteEndOfLifeSummary(list);
    }

    /// <summary>Level 4 — summary counts using LINQ and polymorphic <see cref="Asset"/> list.</summary>
    public void ShowStatistics()
    {
        if (!HasAssets)
        {
            ConsoleUiHelper.WriteWarning("No assets yet — add some or load sample data first.");
            return;
        }

        Console.WriteLine();
        ConsoleUiHelper.WriteTitle("Asset statistics");
        ConsoleUiHelper.WriteSeparator();

        int laptops = _assets.Count(a => a.GetAssetTypeKind() == AssetTypeKind.Laptop);
        int desktops = _assets.Count(a => a.GetAssetTypeKind() == AssetTypeKind.Desktop);
        int phones = _assets.Count(a => a.GetAssetTypeKind() == AssetTypeKind.MobilePhone);

        Console.WriteLine($"  Total assets:     {Count}");
        Console.WriteLine($"  Laptops:          {laptops}");
        Console.WriteLine($"  Desktops:         {desktops}");
        Console.WriteLine($"  Mobile phones:    {phones}");
        Console.WriteLine($"  Total value USD:  {_assets.Sum(a => a.PriceUsd):N2}");
        Console.WriteLine();

        Asset? oldest = _assets.MinBy(a => a.PurchaseDate);
        Asset? newest = _assets.MaxBy(a => a.PurchaseDate);

        if (oldest is not null && newest is not null)
        {
            Console.WriteLine($"  Oldest purchase:  #{oldest.Id} ({oldest.PurchaseDate:yyyy-MM-dd})");
            Console.WriteLine($"  Newest purchase:  #{newest.Id} ({newest.PurchaseDate:yyyy-MM-dd})");
        }

        int yellow = _assets.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.YellowWarning);
        int red = _assets.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.RedWarning);
        int expired = _assets.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.Expired);

        Console.WriteLine();
        Console.WriteLine($"  End-of-life yellow alerts: {yellow}");
        Console.WriteLine($"  End-of-life red alerts:    {red}");
        Console.WriteLine($"  Expired assets:            {expired}");
        ConsoleUiHelper.WriteSeparator();
    }

    /// <summary>Polymorphic print — accepts interface, works for any <see cref="Asset"/> subclass.</summary>
    public void ShowAssetDetails(ICompanyAsset asset)
    {
        Console.WriteLine();
        ConsoleUiHelper.WriteTitle($"Asset details — #{asset.Id}");
        ConsoleUiHelper.WriteInfo($"  {asset.ToDisplayLine()}");
        Console.WriteLine();
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