using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;
using Week45AssetTracking.Services;

// =============================================================================
// Weekly Mini Project — Asset Tracking System
// LEVEL 2 — Sorting + end-of-life warnings (on top of Level 1)
// =============================================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

AssetManager manager = new AssetManager();

ConsoleUiHelper.WriteBanner(
    "Company Asset Tracking — Level 2",
    "Sorted lists and color warnings when assets near their 3-year lifespan");

ConsoleUiHelper.WriteMuted(
    "Tip: type exit at any prompt to quit (EXIT, quit, q — spaces and caps OK).");

if (InputHelper.ReadYesNo("Load sample assets from the assignment (with RED/YELLOW demos)? (y/n): ", defaultYes: true))
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

    manager.ShowSortedWithEndOfLifeHighlights();
    Console.WriteLine();

    if (!InputHelper.ReadYesNo("Continue adding or viewing assets? (y/n): "))
    {
        break;
    }

    Console.WriteLine();
}

if (!InputHelper.ExitRequested)
{
    ConsoleUiHelper.WriteSuccess("Thank you — Level 2 features: LINQ sort + end-of-life colors.");
}

Console.WriteLine();
Console.Write("Press Enter to close...");
Console.ReadLine();

// ----- Add assets (Level 1 flow) -----

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

        Asset? asset = kind == 1 ? ReadComputerFromUser() : ReadMobilePhoneFromUser();
        if (InputHelper.ExitRequested || asset is null)
        {
            return;
        }

        manager.AddAsset(asset);
        ConsoleUiHelper.WriteSuccess("Asset added.");
        Console.WriteLine();

        if (!InputHelper.ReadYesNo("Add another asset in this session? (y/n): "))
        {
            return;
        }

        Console.WriteLine();
    }
}

static Computer? ReadComputerFromUser()
{
    string? brand = InputHelper.ReadLineOrExit("Brand: ");
    if (brand is null)
    {
        return null;
    }

    string? model = InputHelper.ReadLineOrExit("Model: ");
    if (model is null)
    {
        return null;
    }

    DateTime purchaseDate = InputHelper.ReadPurchaseDate();
    if (InputHelper.ExitRequested)
    {
        return null;
    }

    decimal priceUsd = InputHelper.ReadPositiveDecimal("Price (USD): ");
    if (InputHelper.ExitRequested)
    {
        return null;
    }

    decimal priceLocal = InputHelper.ReadPositiveDecimal("Price (local currency, e.g. SEK): ");
    if (InputHelper.ExitRequested)
    {
        return null;
    }

    string? office = InputHelper.ReadLineOrExit("Office (e.g. Sweden): ");
    if (office is null)
    {
        return null;
    }

    Console.WriteLine("Computer form factor: 1 = Laptop, 2 = Desktop");
    int form = InputHelper.ReadChoice(1, 2);
    if (InputHelper.ExitRequested)
    {
        return null;
    }

    return new Computer(brand, model, purchaseDate, priceUsd, priceLocal, office, form == 1);
}

static MobilePhone? ReadMobilePhoneFromUser()
{
    string? brand = InputHelper.ReadLineOrExit("Brand: ");
    if (brand is null)
    {
        return null;
    }

    string? model = InputHelper.ReadLineOrExit("Model: ");
    if (model is null)
    {
        return null;
    }

    DateTime purchaseDate = InputHelper.ReadPurchaseDate();
    if (InputHelper.ExitRequested)
    {
        return null;
    }

    decimal priceUsd = InputHelper.ReadPositiveDecimal("Price (USD): ");
    if (InputHelper.ExitRequested)
    {
        return null;
    }

    decimal priceLocal = InputHelper.ReadPositiveDecimal("Price (local currency): ");
    if (InputHelper.ExitRequested)
    {
        return null;
    }

    string? office = InputHelper.ReadLineOrExit("Office (e.g. USA): ");
    if (office is null)
    {
        return null;
    }

    return new MobilePhone(brand, model, purchaseDate, priceUsd, priceLocal, office);
}