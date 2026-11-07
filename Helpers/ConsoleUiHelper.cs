using Week45AssetTracking.Models;

namespace Week45AssetTracking.Helpers;

/// <summary>
/// Keeps console output readable with a few consistent colors (Level 1+).
/// Colors are always restored so the terminal does not stay highlighted.
/// </summary>
public static class ConsoleUiHelper
{
    public static void WriteTitle(string text) =>
        WriteWithColors(text, ConsoleColor.Cyan, ConsoleColor.Black);

    public static void WriteSuccess(string text) =>
        WriteWithColors(text, ConsoleColor.Green, ConsoleColor.Black);

    public static void WriteInfo(string text) =>
        WriteWithColors(text, ConsoleColor.White, ConsoleColor.DarkBlue);

    public static void WriteWarning(string text) =>
        WriteWithColors(text, ConsoleColor.Black, ConsoleColor.Yellow);

    public static void WriteError(string text) =>
        WriteWithColors(text, ConsoleColor.White, ConsoleColor.DarkRed);

    public static void WriteMuted(string text)
    {
        ConsoleColor previous = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(text);
        Console.ForegroundColor = previous;
    }

    public static void WriteSeparator(char character = '-', int width = 72) =>
        WriteMuted(new string(character, width));

    public static void WriteBanner(string title, string subtitle)
    {
        Console.WriteLine();
        WriteSeparator('=');
        WriteTitle($"  {title}");
        if (!string.IsNullOrWhiteSpace(subtitle))
        {
            WriteMuted($"  {subtitle}");
        }

        WriteSeparator('=');
        Console.WriteLine();
    }

    /// <summary>
    /// Level 2 — prints one asset with yellow/red/magenta styling when end-of-life is near.
    /// </summary>
    public static void WriteAssetLine(Asset asset, int? listIndex = null)
    {
        EndOfLifeStatus status = asset.GetEndOfLifeStatus();
        string prefix = listIndex.HasValue ? $"  {listIndex}. " : "  ";
        string line = prefix + asset.ToDisplayLine();

        string alert = status switch
        {
            EndOfLifeStatus.YellowWarning =>
                "  >> YELLOW: less than 3 months until end-of-life <<",
            EndOfLifeStatus.RedWarning =>
                "  >> RED: less than 6 months until end-of-life <<",
            EndOfLifeStatus.Expired =>
                "  >> EXPIRED: past the 3-year lifespan <<",
            _ => string.Empty
        };

        if (alert.Length > 0)
        {
            line = $"{line}{Environment.NewLine}{alert}";
        }

        switch (status)
        {
            case EndOfLifeStatus.YellowWarning:
                WriteWithColors(line, ConsoleColor.Black, ConsoleColor.Yellow);
                break;
            case EndOfLifeStatus.RedWarning:
                WriteWithColors(line, ConsoleColor.White, ConsoleColor.DarkRed);
                break;
            case EndOfLifeStatus.Expired:
                WriteWithColors(line, ConsoleColor.Yellow, ConsoleColor.DarkMagenta);
                break;
            default:
                WriteInfo(line);
                break;
        }
    }

    private static void WriteWithColors(string text, ConsoleColor foreground, ConsoleColor background)
    {
        ConsoleColor previousForeground = Console.ForegroundColor;
        ConsoleColor previousBackground = Console.BackgroundColor;

        Console.ForegroundColor = foreground;
        Console.BackgroundColor = background;
        Console.WriteLine(text);

        Console.ForegroundColor = previousForeground;
        Console.BackgroundColor = previousBackground;
    }
}