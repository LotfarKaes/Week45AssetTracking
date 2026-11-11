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

    public bool IdExists(int id) => _assets.Any(a => a.Id == id);

    private int AllocateId() => _nextId++;

    /// <summary>Level 5 — rebuild in-memory list from disk (duplicate IDs are rejected).</summary>
    public void ReplaceCatalog(IEnumerable<Asset> assets, int nextId)
    {
        _assets.Clear();
        _assets.AddRange(assets);
        _nextId = Math.Max(1, nextId);

        foreach (Asset asset in _assets)
        {
            _nextId = Math.Max(_nextId, asset.Id + 1);
        }
    }

    public AssetCatalogFile ToCatalogFile()
    {
        AssetCatalogFile catalog = new() { NextId = _nextId };

        foreach (Asset asset in _assets)
        {
            AssetRecord record = new()
            {
                Id = asset.Id,
                Brand = asset.Brand,
                Model = asset.Model,
                PurchaseDate = asset.PurchaseDate,
                PriceUsd = asset.PriceUsd,
                PriceLocal = asset.PriceLocal,
                Office = asset.Office
            };

            switch (asset)
            {
                case Computer computer:
                    record.AssetKind = "Computer";
                    record.IsLaptop = computer.IsLaptop;
                    break;
                case MobilePhone:
                    record.AssetKind = "MobilePhone";
                    break;
                default:
                    record.AssetKind = asset.GetType().Name;
                    break;
            }

            catalog.Assets.Add(record);
        }

        return catalog;
    }

    /// <param name="replaceExisting">True = replace list; false = merge and skip duplicate IDs.</param>
    public void LoadFromCatalog(AssetCatalogFile catalog, bool replaceExisting, out List<string> errors)
    {
        errors = new List<string>();
        List<Asset> imported = new();
        HashSet<int> idsInFile = new();

        foreach (AssetRecord record in catalog.Assets)
        {
            if (!idsInFile.Add(record.Id))
            {
                errors.Add($"Skipped duplicate ID #{record.Id} inside the file.");
                continue;
            }

            if (!replaceExisting && IdExists(record.Id))
            {
                errors.Add($"Skipped ID #{record.Id} — already exists in memory.");
                continue;
            }

            Asset? asset = CreateFromRecord(record);
            if (asset is null)
            {
                errors.Add($"Could not read ID #{record.Id} (unknown type '{record.AssetKind}').");
                continue;
            }

            imported.Add(asset);
        }

        if (replaceExisting)
        {
            ReplaceCatalog(imported, catalog.NextId);
        }
        else
        {
            _assets.AddRange(imported);
            _nextId = Math.Max(_nextId, catalog.NextId);
            foreach (Asset asset in _assets)
            {
                _nextId = Math.Max(_nextId, asset.Id + 1);
            }
        }
    }

    public Asset? CreateFromRecord(AssetRecord record)
    {
        return record.AssetKind switch
        {
            "Computer" => new Computer(
                record.Id,
                record.Brand,
                record.Model,
                record.PurchaseDate,
                record.PriceUsd,
                record.PriceLocal,
                record.Office,
                record.IsLaptop ?? true),
            "MobilePhone" => new MobilePhone(
                record.Id,
                record.Brand,
                record.Model,
                record.PurchaseDate,
                record.PriceUsd,
                record.PriceLocal,
                record.Office),
            _ => null
        };
    }

    /// <summary>
    /// Validates input before add (avoids empty brand/model — same idea as requiring product category).
    /// </summary>
    public bool TryAddComputer(
        string brand,
        string model,
        DateTime purchaseDate,
        decimal priceUsd,
        OfficeLocation office,
        bool isLaptop,
        out Computer? computer,
        out string errorMessage)
    {
        if (!TryValidateAssetFields(brand, model, priceUsd, out errorMessage))
        {
            computer = null;
            return false;
        }

        computer = AddComputer(brand.Trim(), model.Trim(), purchaseDate, priceUsd, office, isLaptop);
        errorMessage = string.Empty;
        return true;
    }

    public bool TryAddMobilePhone(
        string brand,
        string model,
        DateTime purchaseDate,
        decimal priceUsd,
        OfficeLocation office,
        out MobilePhone? phone,
        out string errorMessage)
    {
        if (!TryValidateAssetFields(brand, model, priceUsd, out errorMessage))
        {
            phone = null;
            return false;
        }

        phone = AddMobilePhone(brand.Trim(), model.Trim(), purchaseDate, priceUsd, office);
        errorMessage = string.Empty;
        return true;
    }

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

    private static bool TryValidateAssetFields(
        string brand,
        string model,
        decimal priceUsd,
        out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(brand))
        {
            errorMessage = "Brand cannot be empty — every asset must have a manufacturer name.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(model))
        {
            errorMessage = "Model cannot be empty — please enter the product model.";
            return false;
        }

        if (priceUsd < 0)
        {
            errorMessage = "Price cannot be negative.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
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

    /// <summary>LINQ helper — used by search menu (wired from Program, not dead code).</summary>
    public IReadOnlyList<Asset> FindByBrand(string brandPart)
    {
        string needle = brandPart.Trim();
        return _assets
            .Where(a => a.Brand.Contains(needle, StringComparison.OrdinalIgnoreCase))
            .OrderBy(a => a.Brand, StringComparer.OrdinalIgnoreCase)
            .ThenBy(a => a.Model, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>LINQ helper — used by search menu.</summary>
    public IReadOnlyList<Asset> FindByModel(string modelPart)
    {
        string needle = modelPart.Trim();
        return _assets
            .Where(a => a.Model.Contains(needle, StringComparison.OrdinalIgnoreCase))
            .OrderBy(a => a.Model, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>LINQ helper — assets for one office (Level 3).</summary>
    public IReadOnlyList<Asset> GetAssetsByOffice(OfficeLocation office) =>
        _assets.Where(a => a.Office == office).OrderBy(a => a.PurchaseDate).ToList();

    /// <summary>Sort by purchase price in USD (low → high), like Product List sort by price.</summary>
    public IEnumerable<Asset> GetSortedByPriceUsd() =>
        _assets.OrderBy(a => a.PriceUsd).ThenBy(a => a.Brand);

    public decimal CalculateTotalPriceUsd() => _assets.Sum(a => a.PriceUsd);

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

        decimal totalUsd = list.Sum(a => a.PriceUsd);
        ConsoleUiHelper.WriteSuccess(
            $"Total value (USD) for this list: {totalUsd:N2}");
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

        ConsoleUiHelper.WriteStatLine("Total assets:", Count);
        ConsoleUiHelper.WriteStatLine("Laptops:", laptops);
        ConsoleUiHelper.WriteStatLine("Desktops:", desktops);
        ConsoleUiHelper.WriteStatLine("Mobile phones:", phones);
        ConsoleUiHelper.WriteStatLine("Total value (USD):", $"{CalculateTotalPriceUsd():N2}");
        Console.WriteLine();

        Asset? oldest = _assets.MinBy(a => a.PurchaseDate);
        Asset? newest = _assets.MaxBy(a => a.PurchaseDate);

        if (oldest is not null && newest is not null)
        {
            ConsoleUiHelper.WriteStatLine("Oldest purchase:", $"#{oldest.Id} ({oldest.PurchaseDate:yyyy-MM-dd})");
            ConsoleUiHelper.WriteStatLine("Newest purchase:", $"#{newest.Id} ({newest.PurchaseDate:yyyy-MM-dd})");
        }

        int yellow = _assets.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.YellowWarning);
        int red = _assets.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.RedWarning);
        int expired = _assets.Count(a => a.GetEndOfLifeStatus() == EndOfLifeStatus.Expired);

        Console.WriteLine();
        ConsoleUiHelper.WriteWarning($"End-of-life yellow alerts: {yellow}");
        ConsoleUiHelper.WriteWarning($"End-of-life red alerts:    {red}");
        ConsoleUiHelper.WriteError($"Expired assets:            {expired}");
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