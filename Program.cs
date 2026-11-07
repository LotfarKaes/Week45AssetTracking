using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;
using Week45AssetTracking.Services;

// =============================================================================
// Weekly Mini Project — Asset Tracking System
// LEVEL 3 — Multi-office + currency (on top of Levels 1–2)
// =============================================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

CurrencyService currencyService = new();
AssetManager manager = new AssetManager(currencyService);

ConsoleUiHelper.WriteBanner(
    "Company Asset Tracking — Level 3",
    "Offices in Sweden, USA, and Turkey — USD prices converted to local currency");

ConsoleUiHelper.WriteMuted(
    "Tip: type exit at any prompt to quit (EXIT, quit, q — spaces and caps OK).");

currencyService.TryFetchLiveRates();
Console.WriteLine();

if (InputHelper.ReadYesNo("Load sample assets from the assignment? (y/n): ", defaultYes: true))
{
    manager.SeedSampleData();
    Console.WriteLine();
}

while (!InputHelper.ExitRequested)
{
    RunAddAssetSession(manager);

    if (InputHelper.ExitRequested)
    {
        break;
    }

    ShowListViewMenu(manager);
    Console.WriteLine();

    if (InputHelper.ReadYesNo("Refresh live currency rates and update local prices? (y/n): "))
    {
        currencyService.TryFetchLiveRates();
        manager.RecalculateAllLocalPrices();
        ConsoleUiHelper.WriteSuccess("All local prices updated from USD.");
        Console.WriteLine();
    }

    if (!InputHelper.ReadYesNo("Continue adding assets or viewing lists? (y/n): "))
    {
        break;
    }

    Console.WriteLine();
}

if (!InputHelper.ExitRequested)
{
    ConsoleUiHelper.WriteSuccess("Thank you — Level 3: offices, currency conversion, office sorting.");
}

Console.WriteLine();
Console.Write("Press Enter to close...");
Console.ReadLine();

// ----- Views -----

static void ShowListViewMenu(AssetManager manager)
{
    ConsoleUiHelper.WriteTitle("How would you like to view the register?");
    Console.WriteLine("  1 - Sort by asset type → purchase date (Level 2)");
    Console.WriteLine("  2 - Sort by office → purchase date (Level 3)");
    Console.WriteLine("  3 - Show both lists");
    Console.WriteLine();

    int choice = InputHelper.ReadChoice(1, 3);
    if (InputHelper.ExitRequested)
    {
        return;
    }

    switch (choice)
    {
        case 1:
            manager.ShowSortedWithEndOfLifeHighlights(
                manager.GetSortedByTypeThenDate(),
                "Sorted by asset type, then purchase date");
            break;
        case 2:
            manager.ShowSortedWithEndOfLifeHighlights(
                manager.GetSortedByOfficeThenDate(),
                "Sorted by office, then purchase date");
            break;
        case 3:
            manager.ShowSortedWithEndOfLifeHighlights(
                manager.GetSortedByTypeThenDate(),
                "Sorted by asset type, then purchase date");
            manager.ShowSortedWithEndOfLifeHighlights(
                manager.GetSortedByOfficeThenDate(),
                "Sorted by office, then purchase date");
            break;
    }
}

// ----- Add assets -----

static void RunAddAssetSession(AssetManager manager)
{
    while (!InputHelper.ExitRequested)
    {
        ConsoleUiHelper.WriteTitle("Add a new asset");
        Console.WriteLine("  1 - Computer (laptop or desktop)");
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
            if (!TryAddComputer(manager))
            {
                return;
            }
        }
        else
        {
            if (!TryAddMobilePhone(manager))
            {
                return;
            }
        }

        Console.WriteLine();

        if (!InputHelper.ReadYesNo("Add another asset in this session? (y/n): "))
        {
            return;
        }

        Console.WriteLine();
    }
}

static bool TryAddComputer(AssetManager manager)
{
    string? brand = InputHelper.ReadLineOrExit("Brand: ");
    if (brand is null)
    {
        return false;
    }

    string? model = InputHelper.ReadLineOrExit("Model: ");
    if (model is null)
    {
        return false;
    }

    DateTime purchaseDate = InputHelper.ReadPurchaseDate();
    if (InputHelper.ExitRequested)
    {
        return false;
    }

    decimal priceUsd = InputHelper.ReadPositiveDecimal("Price in USD (local currency is calculated): ");
    if (InputHelper.ExitRequested)
    {
        return false;
    }

    OfficeLocation office = InputHelper.ReadOfficeLocation();
    if (InputHelper.ExitRequested)
    {
        return false;
    }

    Console.WriteLine("Computer form factor: 1 = Laptop, 2 = Desktop");
    int form = InputHelper.ReadChoice(1, 2);
    if (InputHelper.ExitRequested)
    {
        return false;
    }

    Computer computer = manager.AddComputer(
        brand, model, purchaseDate, priceUsd, office, isLaptop: form == 1);

    ConsoleUiHelper.WriteSuccess(
        $"Asset added. Local price: {computer.LocalCurrencyCode} {computer.PriceLocal:N2}");

    return true;
}

static bool TryAddMobilePhone(AssetManager manager)
{
    string? brand = InputHelper.ReadLineOrExit("Brand: ");
    if (brand is null)
    {
        return false;
    }

    string? model = InputHelper.ReadLineOrExit("Model: ");
    if (model is null)
    {
        return false;
    }

    DateTime purchaseDate = InputHelper.ReadPurchaseDate();
    if (InputHelper.ExitRequested)
    {
        return false;
    }

    decimal priceUsd = InputHelper.ReadPositiveDecimal("Price in USD (local currency is calculated): ");
    if (InputHelper.ExitRequested)
    {
        return false;
    }

    OfficeLocation office = InputHelper.ReadOfficeLocation();
    if (InputHelper.ExitRequested)
    {
        return false;
    }

    MobilePhone phone = manager.AddMobilePhone(brand, model, purchaseDate, priceUsd, office);

    ConsoleUiHelper.WriteSuccess(
        $"Asset added. Local price: {phone.LocalCurrencyCode} {phone.PriceLocal:N2}");

    return true;
}