using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;
using Week45AssetTracking.Services;

// =============================================================================
// Weekly Mini Project — Asset Tracking System
// LEVEL 1 — Basic classes, objects, constructors, List<Asset>
// =============================================================================

Console.OutputEncoding = System.Text.Encoding.UTF8;

AssetManager manager = new AssetManager();

ConsoleUiHelper.WriteBanner(
    "Company Asset Tracking — Level 1",
    "Add computers and mobile phones, then view your list");

ConsoleUiHelper.WriteMuted(
    "Tip: type exit at any prompt to quit (works as EXIT, quit, q, with spaces too).");
Console.WriteLine();

while (!InputHelper.ExitRequested)
{
    ConsoleUiHelper.WriteTitle("Add a new asset");
    Console.WriteLine("  1 - Computer (laptop or desktop)");
    Console.WriteLine("  2 - Mobile phone");
    Console.WriteLine();

    int kind = InputHelper.ReadChoice(1, 2);
    if (InputHelper.ExitRequested)
    {
        break;
    }

    Console.WriteLine();

    Asset? asset = kind == 1 ? ReadComputerFromUser() : ReadMobilePhoneFromUser();
    if (InputHelper.ExitRequested || asset is null)
    {
        break;
    }

    manager.AddAsset(asset);
    ConsoleUiHelper.WriteSuccess("Asset added to the list.");
    Console.WriteLine();

    if (!InputHelper.ReadYesNo("Add another asset now? (y/n): "))
    {
        break;
    }

    Console.WriteLine();
}

if (!InputHelper.ExitRequested)
{
    manager.ShowAllAssets();
    Console.WriteLine();
    ConsoleUiHelper.WriteSuccess("Level 1 complete — assets are stored in List<Asset> via AssetManager.");
}

Console.WriteLine();
Console.Write("Press Enter to close...");
Console.ReadLine();

// ----- Read one asset from the console -----

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

    bool isLaptop = form == 1;

    return new Computer(brand, model, purchaseDate, priceUsd, priceLocal, office, isLaptop);
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