using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;
using Week45AssetTracking.Services;

// =============================================================================
// Weekly Mini Project — Asset Tracking System
// LEVEL 5 — JSON save/load, auto-save, export (Levels 1–4 included)
// =============================================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

CurrencyService currencyService = new();
AssetManager manager = new AssetManager(currencyService);
AssetFileService fileService = new AssetFileService();

ConsoleUiHelper.WriteBanner(
    "Company Asset Tracking — Level 5",
    "Full menu + assets.json persistence and text report export");

ConsoleUiHelper.WriteMuted("Tip: type exit at any prompt (EXIT, quit, q — any spacing).");

currencyService.TryFetchLiveRates();
Console.WriteLine();

TryLoadOnStartup(manager, fileService);

if (!manager.HasAssets &&
    InputHelper.ReadYesNo("Load sample assets (IDs #1–#8)? (y/n): ", defaultYes: true))
{
    manager.SeedSampleData();
    AutoSave(manager, fileService);
    Console.WriteLine();
}

bool running = true;

while (running && !InputHelper.ExitRequested)
{
    PrintMainMenu();
    int choice = InputHelper.ReadMainMenuChoice(0, 12);

    if (InputHelper.ExitRequested || choice == 0)
    {
        running = false;
        continue;
    }

    Console.WriteLine();

    switch (choice)
    {
        case 1:
            HandleAddAsset(manager, fileService);
            break;
        case 2:
            manager.ShowSortedWithEndOfLifeHighlights(
                manager.GetSortedByTypeThenDate(),
                "Sorted by asset type, then purchase date");
            InputHelper.PressEnterToContinue();
            break;
        case 3:
            manager.ShowSortedWithEndOfLifeHighlights(
                manager.GetSortedByOfficeThenDate(),
                "Sorted by office, then purchase date");
            InputHelper.PressEnterToContinue();
            break;
        case 4:
            HandleSearch(manager);
            break;
        case 5:
            HandleViewById(manager);
            break;
        case 6:
            HandleEditById(manager, fileService);
            break;
        case 7:
            HandleRemoveById(manager, fileService);
            break;
        case 8:
            manager.ShowStatistics();
            InputHelper.PressEnterToContinue();
            break;
        case 9:
            currencyService.TryFetchLiveRates();
            manager.RecalculateAllLocalPrices();
            AutoSave(manager, fileService);
            ConsoleUiHelper.WriteSuccess("Local prices recalculated from USD.");
            InputHelper.PressEnterToContinue();
            break;
        case 10:
            TrySaveCatalog(manager, fileService);
            InputHelper.PressEnterToContinue();
            break;
        case 11:
            TryLoadCatalog(manager, fileService, replaceExisting: true);
            InputHelper.PressEnterToContinue();
            break;
        case 12:
            HandleExportReport(manager, fileService);
            InputHelper.PressEnterToContinue();
            break;
    }
}

if (!InputHelper.ExitRequested)
{
    AutoSave(manager, fileService);
    ConsoleUiHelper.WriteSuccess("Thank you for using Asset Tracking. Goodbye!");
}

Console.WriteLine();
Console.Write("Press Enter to close...");
Console.ReadLine();

// ----- File helpers (Level 5) -----

static void TryLoadOnStartup(AssetManager manager, AssetFileService fileService)
{
    if (!fileService.FileExists())
    {
        return;
    }

    if (InputHelper.ReadYesNo($"Found {AssetFileService.DefaultFileName}. Load saved assets? (y/n): "))
    {
        TryLoadCatalog(manager, fileService, replaceExisting: true);
    }

    Console.WriteLine();
}

static void TrySaveCatalog(AssetManager manager, AssetFileService fileService)
{
    try
    {
        AssetCatalogFile catalog = manager.ToCatalogFile();
        fileService.Save(catalog);
        ConsoleUiHelper.WriteSuccess(
            $"Saved {catalog.Assets.Count} asset(s) to {AssetFileService.DefaultFileName}.");
    }
    catch (Exception ex)
    {
        ConsoleUiHelper.WriteError($"Save failed: {ex.Message}");
    }
}

static void TryLoadCatalog(AssetManager manager, AssetFileService fileService, bool replaceExisting)
{
    if (!fileService.FileExists())
    {
        ConsoleUiHelper.WriteWarning(
            $"No {AssetFileService.DefaultFileName} yet — use menu 10 to save first.");
        return;
    }

    try
    {
        AssetCatalogFile catalog = fileService.Load();
        int countBefore = manager.Count;

        manager.LoadFromCatalog(catalog, replaceExisting, out List<string> errors);

        int loaded = replaceExisting ? manager.Count : manager.Count - countBefore;

        if (loaded > 0 || (replaceExisting && catalog.Assets.Count == 0))
        {
            ConsoleUiHelper.WriteSuccess(
                $"Catalog loaded — {manager.Count} asset(s) in memory. Next ID: {manager.NextId}.");
        }
        else
        {
            ConsoleUiHelper.WriteWarning("Nothing new was imported.");
        }

        foreach (string error in errors)
        {
            ConsoleUiHelper.WriteWarning(error);
        }
    }
    catch (Exception ex)
    {
        ConsoleUiHelper.WriteError($"Load failed: {ex.Message}");
    }
}

static void AutoSave(AssetManager manager, AssetFileService fileService)
{
    try
    {
        fileService.Save(manager.ToCatalogFile());
        ConsoleUiHelper.WriteMuted($"(Auto-saved to {AssetFileService.DefaultFileName})");
    }
    catch
    {
        ConsoleUiHelper.WriteWarning("Auto-save failed — use menu 10 to save manually.");
    }
}

static void HandleExportReport(AssetManager manager, AssetFileService fileService)
{
    if (!manager.HasAssets)
    {
        ConsoleUiHelper.WriteWarning("Add or load assets before exporting a report.");
        return;
    }

    List<Asset> sorted = manager.GetSortedByOfficeThenDate().ToList();
    fileService.ExportReport(sorted);
}

// ----- Menu -----

static void PrintMainMenu()
{
    Console.WriteLine();
    ConsoleUiHelper.WriteTitle("Main menu");
    Console.WriteLine("  1  - Add asset (computer or phone)");
    Console.WriteLine("  2  - View all — sort by type → date      [Level 2]");
    Console.WriteLine("  3  - View all — sort by office → date    [Level 3]");
    Console.WriteLine("  4  - Search by brand or model");
    Console.WriteLine("  5  - View asset details by ID            [Level 4]");
    Console.WriteLine("  6  - Edit asset by ID");
    Console.WriteLine("  7  - Remove asset by ID");
    Console.WriteLine("  8  - Statistics dashboard");
    Console.WriteLine("  9  - Refresh currency rates");
    Console.WriteLine(" 10  - Save catalog to JSON               [Level 5]");
    Console.WriteLine(" 11  - Load catalog from JSON");
    Console.WriteLine(" 12  - Export text report");
    Console.WriteLine("  0  - Exit (auto-saves on quit)");
    Console.WriteLine();
}

// ----- Handlers -----

static void HandleAddAsset(AssetManager manager, AssetFileService fileService)
{
    ConsoleUiHelper.WriteTitle("Add asset");
    Console.WriteLine("  1 - Computer (laptop / desktop)");
    Console.WriteLine("  2 - Mobile phone");
    Console.WriteLine();

    int kind = InputHelper.ReadChoice(1, 2);
    if (InputHelper.ExitRequested)
    {
        return;
    }

    Console.WriteLine();

    if (kind == 1)
    {
        TryAddComputer(manager, fileService);
    }
    else
    {
        TryAddMobilePhone(manager, fileService);
    }
}

static void TryAddComputer(AssetManager manager, AssetFileService fileService)
{
    string? brand = InputHelper.ReadLineOrExit("Brand: ");
    if (brand is null)
    {
        return;
    }

    string? model = InputHelper.ReadLineOrExit("Model: ");
    if (model is null)
    {
        return;
    }

    DateTime purchaseDate = InputHelper.ReadPurchaseDate();
    if (InputHelper.ExitRequested)
    {
        return;
    }

    decimal priceUsd = InputHelper.ReadPositiveDecimal("Price in USD: ");
    if (InputHelper.ExitRequested)
    {
        return;
    }

    OfficeLocation office = InputHelper.ReadOfficeLocation();
    if (InputHelper.ExitRequested)
    {
        return;
    }

    Console.WriteLine("  1 = Laptop,  2 = Desktop");
    int form = InputHelper.ReadChoice(1, 2);
    if (InputHelper.ExitRequested)
    {
        return;
    }

    Computer computer = manager.AddComputer(
        brand, model, purchaseDate, priceUsd, office, isLaptop: form == 1);

    ConsoleUiHelper.WriteSuccess(
        $"Saved as #{computer.Id}. Local: {computer.LocalCurrencyCode} {computer.PriceLocal:N2} | Age: {computer.AgeDescription}");

    AutoSave(manager, fileService);
}

static void TryAddMobilePhone(AssetManager manager, AssetFileService fileService)
{
    string? brand = InputHelper.ReadLineOrExit("Brand: ");
    if (brand is null)
    {
        return;
    }

    string? model = InputHelper.ReadLineOrExit("Model: ");
    if (model is null)
    {
        return;
    }

    DateTime purchaseDate = InputHelper.ReadPurchaseDate();
    if (InputHelper.ExitRequested)
    {
        return;
    }

    decimal priceUsd = InputHelper.ReadPositiveDecimal("Price in USD: ");
    if (InputHelper.ExitRequested)
    {
        return;
    }

    OfficeLocation office = InputHelper.ReadOfficeLocation();
    if (InputHelper.ExitRequested)
    {
        return;
    }

    MobilePhone phone = manager.AddMobilePhone(brand, model, purchaseDate, priceUsd, office);

    ConsoleUiHelper.WriteSuccess(
        $"Saved as #{phone.Id}. Local: {phone.LocalCurrencyCode} {phone.PriceLocal:N2} | Age: {phone.AgeDescription}");

    AutoSave(manager, fileService);
}

static void HandleSearch(AssetManager manager)
{
    string? term = InputHelper.ReadLineOrExit("Search (brand or model): ");
    if (term is null)
    {
        return;
    }

    manager.ShowSortedWithEndOfLifeHighlights(
        manager.SearchByBrandOrModel(term),
        $"Search results for \"{term}\"");

    InputHelper.PressEnterToContinue();
}

static void HandleViewById(AssetManager manager)
{
    int id = InputHelper.ReadAssetId("Asset ID: ");
    if (InputHelper.ExitRequested)
    {
        return;
    }

    Asset? asset = manager.FindById(id);
    if (asset is null)
    {
        ConsoleUiHelper.WriteError($"No asset with ID #{id}. Check the list and try again.");
        InputHelper.PressEnterToContinue();
        return;
    }

    manager.ShowAssetDetails(asset);
    InputHelper.PressEnterToContinue();
}

static void HandleEditById(AssetManager manager, AssetFileService fileService)
{
    int id = InputHelper.ReadAssetId("Asset ID to edit: ");
    if (InputHelper.ExitRequested)
    {
        return;
    }

    Asset? asset = manager.FindById(id);
    if (asset is null)
    {
        ConsoleUiHelper.WriteError($"No asset with ID #{id}.");
        InputHelper.PressEnterToContinue();
        return;
    }

    ConsoleUiHelper.WriteInfo($"Editing #{asset.Id} — press Enter to keep each field.");
    Console.WriteLine();

    string? brand = InputHelper.ReadLineOrExit($"Brand [{asset.Brand}]: ", allowEmpty: true);
    if (brand is null)
    {
        return;
    }

    if (!string.IsNullOrWhiteSpace(brand))
    {
        asset.Brand = brand.Trim();
    }

    string? model = InputHelper.ReadLineOrExit($"Model [{asset.Model}]: ", allowEmpty: true);
    if (model is null)
    {
        return;
    }

    if (!string.IsNullOrWhiteSpace(model))
    {
        asset.Model = model.Trim();
    }

    string? dateText = InputHelper.ReadLineOrExit(
        $"Purchase date [{asset.PurchaseDate:yyyy-MM-dd}]: ",
        allowEmpty: true);
    if (dateText is null)
    {
        return;
    }

    if (!string.IsNullOrWhiteSpace(dateText) && DateTime.TryParse(dateText, out DateTime newDate))
    {
        asset.PurchaseDate = newDate.Date;
    }

    string? usdText = InputHelper.ReadLineOrExit($"Price USD [{asset.PriceUsd}]: ", allowEmpty: true);
    if (usdText is null)
    {
        return;
    }

    if (!string.IsNullOrWhiteSpace(usdText) && decimal.TryParse(usdText, out decimal newUsd))
    {
        asset.PriceUsd = newUsd;
        asset.PriceLocal = manager.CurrencyService.ConvertFromUsd(newUsd, asset.Office);
    }

    ConsoleUiHelper.WriteSuccess($"Asset #{asset.Id} updated. Age is now {asset.AgeDescription}.");
    AutoSave(manager, fileService);
    InputHelper.PressEnterToContinue();
}

static void HandleRemoveById(AssetManager manager, AssetFileService fileService)
{
    int id = InputHelper.ReadAssetId("Asset ID to remove: ");
    if (InputHelper.ExitRequested)
    {
        return;
    }

    Asset? asset = manager.FindById(id);
    if (asset is null)
    {
        ConsoleUiHelper.WriteError($"No asset with ID #{id}.");
        InputHelper.PressEnterToContinue();
        return;
    }

    ConsoleUiHelper.WriteMuted(asset.ToDisplayLine());
    if (!InputHelper.ReadYesNo($"Remove asset #{id}? (y/n): "))
    {
        ConsoleUiHelper.WriteMuted("Cancelled — nothing removed.");
        InputHelper.PressEnterToContinue();
        return;
    }

    manager.RemoveById(id);
    ConsoleUiHelper.WriteSuccess($"Asset #{id} removed.");
    AutoSave(manager, fileService);
    InputHelper.PressEnterToContinue();
}