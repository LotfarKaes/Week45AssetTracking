using System.Xml;
using Week45AssetTracking.Helpers;
using Week45AssetTracking.Models;

namespace Week45AssetTracking.Services;

/// <summary>
/// Level 3 — converts USD purchase prices into each office local currency.
/// Tries live ECB rates first (course example), then uses built-in fallback rates.
/// </summary>
public class CurrencyService
{
    private readonly Dictionary<string, decimal> _rateFromEur = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Dictionary<string, decimal> FallbackFromUsd = new(StringComparer.OrdinalIgnoreCase)
    {
        ["USD"] = 1.00m,
        ["SEK"] = 10.50m,
        ["TRY"] = 34.00m,
        ["EUR"] = 0.92m
    };

    public bool UsingLiveRates { get; private set; }

    /// <summary>Downloads ECB daily XML (same source as LiveCurrency.cs in the course materials).</summary>
    public void TryFetchLiveRates()
    {
        const string url = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        try
        {
            _rateFromEur.Clear();
            _rateFromEur["EUR"] = 1.0m;

            using XmlTextReader reader = new(url);
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element || reader.Name != "Cube")
                {
                    continue;
                }

                string? code = reader.GetAttribute("currency");
                string? rateText = reader.GetAttribute("rate");

                if (code is not null && rateText is not null && decimal.TryParse(rateText, out decimal rate))
                {
                    _rateFromEur[code] = rate;
                }
            }

            UsingLiveRates = _rateFromEur.Count > 1;
            if (UsingLiveRates)
            {
                ConsoleUiHelper.WriteSuccess("Live exchange rates loaded (European Central Bank).");
            }
        }
        catch (Exception)
        {
            UsingLiveRates = false;
            ConsoleUiHelper.WriteWarning(
                "Could not download live rates — using built-in fallback exchange rates instead.");
        }
    }

    public decimal ConvertFromUsd(decimal amountUsd, OfficeLocation office)
    {
        string targetCurrency = OfficeInfo.GetCurrencyCode(office);
        return Convert(amountUsd, "USD", targetCurrency);
    }

    /// <summary>Converts between ISO currency codes via EUR when live data exists.</summary>
    public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
    {
        fromCurrency = fromCurrency.Trim().ToUpperInvariant();
        toCurrency = toCurrency.Trim().ToUpperInvariant();

        if (fromCurrency == toCurrency)
        {
            return amount;
        }

        if (UsingLiveRates &&
            _rateFromEur.ContainsKey(fromCurrency) &&
            _rateFromEur.ContainsKey(toCurrency))
        {
            decimal inEur = fromCurrency == "EUR" ? amount : amount / _rateFromEur[fromCurrency];
            return toCurrency == "EUR" ? inEur : inEur * _rateFromEur[toCurrency];
        }

        if (!FallbackFromUsd.TryGetValue(fromCurrency, out decimal fromRate) ||
            !FallbackFromUsd.TryGetValue(toCurrency, out decimal toRate))
        {
            return amount;
        }

        decimal usd = fromCurrency == "USD" ? amount : amount / fromRate;
        return toCurrency == "USD" ? usd : usd * toRate;
    }
}