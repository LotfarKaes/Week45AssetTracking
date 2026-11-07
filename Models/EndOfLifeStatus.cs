namespace Week45AssetTracking.Models;

/// <summary>
/// End-of-life warning level — company assets live for 3 years (Level 2).
/// </summary>
public enum EndOfLifeStatus
{
    /// <summary>More than 6 months before end-of-life — normal line, no alert color.</summary>
    Normal,

    /// <summary>Less than 6 months remaining (but at least 3 months) — red highlight.</summary>
    RedWarning,

    /// <summary>Less than 3 months remaining — yellow highlight (most urgent).</summary>
    YellowWarning,

    /// <summary>Past the 3-year lifespan.</summary>
    Expired
}