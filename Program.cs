using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;
using Week45AssetTracking.Services;

// =============================================================================
// Weekly Mini Project — Asset Tracking System
// LEVEL 4 — IDs, age, interface, menu (Levels 1–3 included)
// =============================================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

CurrencyService currencyService = new();
AssetManager manager = new AssetManager(currencyService);

ConsoleUiHelper.WriteBanner(
    "Company Asset Tracking — Level 4",
    "Menu-driven app with unique IDs, age tracking, and full OOP design");

ConsoleUiHelper.WriteMuted("Tip: type exit at any prompt (EXIT, quit, q — any spacing).");

currencyService.TryFetchLiveRates();
Console.WriteLine();

if (InputHelper.ReadYesNo("Load sample assets (IDs #1–#8 with EOL demos)? (y/n): ", defaultYes: true))
{
    manager.SeedSampleData();
    Console.WriteLine();
}

bool running = true;

while (running && !InputHelper.ExitRequested)
{
    PrintMainMenu();
    int choice = InputHelper.ReadMainMenuChoice(0, 9);

    if (InputHelper.ExitRequested || choice == 0)
    {
        running = false;
        continue;
    }

    Console.WriteLine();

    switch (choice)
    {
        case 1:
            HandleAddAsset(manager);
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
            HandleEditById(manager);
            break;
        case 7:
            HandleRemoveById(manager);
            break;
        case 8:
            manager.ShowStatistics();
            InputHelper.PressEnterToContinue();
            break;
        case 9:
            currencyService.TryFetchLiveRates();
            manager.RecalculateAllLocalPrices();
            ConsoleUiHelper.WriteSuccess("Local prices recalculated from USD.");
            InputHelper.PressEnterToContinue();
            break;
    }
}

if (!InputHelper.ExitRequested)
{
    ConsoleUiHelper.WriteSuccess("Thank you for using Asset Tracking. Goodbye!");
}

Console.WriteLine();
Console.Write("Press Enter to close...");
Console.ReadLine();

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
    Console.WriteLine("  0  - Exit");
    Console.WriteLine();
}

// ----- Handlers -----

static void HandleAddAsset(AssetManager manager)
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
        TryAddComputer(manager);
    }
    else
    {
        TryAddMobilePhone(manager);
    }
}

static void TryAddComputer(AssetManager manager)
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
}

static void TryAddMobilePhone(AssetManager manager)
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

static void HandleEditById(AssetManager manager)
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
    InputHelper.PressEnterToContinue();
}

static void HandleRemoveById(AssetManager manager)
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
    InputHelper.PressEnterToContinue();
}