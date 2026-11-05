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