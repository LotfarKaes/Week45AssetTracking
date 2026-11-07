using Week45AssetTracking.Models;

namespace Week45AssetTracking.Helpers;

/// <summary>
/// Reads and validates user input. Typing exit (any common format) stops the program safely.
/// </summary>
public static class InputHelper
{
    /// <summary>True after the user asked to quit with exit / quit / q.</summary>
    public static bool ExitRequested { get; private set; }

    /// <summary>
    /// Detects exit commands: "exit", "  EXIT  ", "quit", "q", "avsluta" (spaces removed, case ignored).
    /// </summary>
    public static bool IsExitCommand(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        string normalized = input.Trim().ToLowerInvariant();
        normalized = normalized.Replace(" ", string.Empty, StringComparison.Ordinal);

        return normalized is "exit" or "quit" or "q" or "avsluta";
    }

    /// <summary>Reads one line; returns null when the user typed exit.</summary>
    public static string? ReadLineOrExit(string prompt, bool allowEmpty = false)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (IsExitCommand(input))
            {
                ExitRequested = true;
                ConsoleUiHelper.WriteInfo("Closing the app. See you next time!");
                return null;
            }

            string value = input?.Trim() ?? string.Empty;

            if (!allowEmpty && string.IsNullOrWhiteSpace(value))
            {
                ConsoleUiHelper.WriteWarning("That cannot be empty. Try again (or type exit to quit).");
                continue;
            }

            return value;
        }
    }

    public static int ReadChoice(int minInclusive, int maxInclusive)
    {
        while (true)
        {
            Console.Write($"Your choice ({minInclusive}-{maxInclusive}, or exit): ");
            string? input = Console.ReadLine();

            if (IsExitCommand(input))
            {
                ExitRequested = true;
                return -1;
            }

            string choice = input?.Trim() ?? string.Empty;

            if (int.TryParse(choice, out int value) && value >= minInclusive && value <= maxInclusive)
            {
                return value;
            }

            ConsoleUiHelper.WriteWarning(
                $"Please enter a number from {minInclusive} to {maxInclusive}, or type exit.");
        }
    }

    public static decimal ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            string? text = ReadLineOrExit(prompt);
            if (text is null)
            {
                return -1;
            }

            if (!decimal.TryParse(text, out decimal value))
            {
                ConsoleUiHelper.WriteWarning("That is not a valid number. Example: 1500 or 999.50");
                continue;
            }

            if (value < 0)
            {
                ConsoleUiHelper.WriteWarning("Amount cannot be negative. Please enter zero or higher.");
                continue;
            }

            return value;
        }
    }

    public static DateTime ReadPurchaseDate()
    {
        while (true)
        {
            ConsoleUiHelper.WriteMuted("Format: yyyy-MM-dd (example from assignment: 2024-03-15)");
            string? text = ReadLineOrExit("Purchase date: ");
            if (text is null)
            {
                return DateTime.MinValue;
            }

            if (DateTime.TryParse(text, out DateTime date))
            {
                if (date.Date > DateTime.Today)
                {
                    ConsoleUiHelper.WriteWarning("Purchase date cannot be in the future.");
                    continue;
                }

                return date.Date;
            }

            ConsoleUiHelper.WriteWarning("Invalid date. Use yyyy-MM-dd.");
        }
    }

    public static bool ReadYesNo(string prompt, bool defaultYes = false)
    {
        while (true)
        {
            string? text = ReadLineOrExit(prompt, allowEmpty: true);
            if (text is null)
            {
                return false;
            }

            if (text.Length == 0)
            {
                return defaultYes;
            }

            string answer = text.ToLowerInvariant();
            if (answer is "y" or "yes" or "j" or "ja")
            {
                return true;
            }

            if (answer is "n" or "no" or "nej")
            {
                return false;
            }

            ConsoleUiHelper.WriteWarning("Please answer y/yes or n/no (or exit).");
        }
    }

    /// <summary>Level 3 — pick Sweden, USA, or Turkey (by number or name).</summary>
    public static OfficeLocation ReadOfficeLocation()
    {
        while (true)
        {
            OfficeInfo.WriteOfficeMenu();
            string? text = ReadLineOrExit("Office (1-3 or country name): ");
            if (text is null)
            {
                return default;
            }

            OfficeLocation? office = OfficeInfo.TryParse(text);
            if (office.HasValue)
            {
                return office.Value;
            }

            ConsoleUiHelper.WriteWarning("Please choose 1, 2, 3, or type Sweden / USA / Turkey.");
        }
    }

    public static void PressEnterToContinue()
    {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
        Console.WriteLine();
    }
}