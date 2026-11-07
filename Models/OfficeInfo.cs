namespace Week45AssetTracking.Models;

/// <summary>
/// Maps each <see cref="OfficeLocation"/> to a display name and currency code (Level 3).
/// </summary>
public static class OfficeInfo
{
    private static readonly Dictionary<OfficeLocation, (string Name, string Currency)> Details = new()
    {
        [OfficeLocation.Sweden] = ("Sweden", "SEK"),
        [OfficeLocation.Usa] = ("USA", "USD"),
        [OfficeLocation.Turkey] = ("Turkey", "TRY")
    };

    public static string GetDisplayName(OfficeLocation office) => Details[office].Name;

    public static string GetCurrencyCode(OfficeLocation office) => Details[office].Currency;

    public static void WriteOfficeMenu()
    {
        Console.WriteLine("  1 - Sweden (SEK)");
        Console.WriteLine("  2 - USA (USD)");
        Console.WriteLine("  3 - Turkey (TRY)");
    }

    /// <summary>
    /// Parses "1", "sweden", "SEK", etc. Returns null when input does not match.
    /// </summary>
    public static OfficeLocation? TryParse(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        string text = input.Trim();

        if (int.TryParse(text, out int number) && number is >= 1 and <= 3)
        {
            return number switch
            {
                1 => OfficeLocation.Sweden,
                2 => OfficeLocation.Usa,
                3 => OfficeLocation.Turkey,
                _ => null
            };
        }

        text = text.Replace(" ", string.Empty, StringComparison.Ordinal);

        foreach (KeyValuePair<OfficeLocation, (string Name, string Currency)> entry in Details)
        {
            if (entry.Value.Name.Equals(text, StringComparison.OrdinalIgnoreCase) ||
                entry.Value.Currency.Equals(text, StringComparison.OrdinalIgnoreCase) ||
                entry.Key.ToString().Equals(text, StringComparison.OrdinalIgnoreCase))
            {
                return entry.Key;
            }
        }

        return null;
    }
}