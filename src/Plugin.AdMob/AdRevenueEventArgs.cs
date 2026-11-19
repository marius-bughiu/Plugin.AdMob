namespace Plugin.AdMob;

/// <summary>
/// Revenue reported by Google Mobile Ads for an individual impression.
/// </summary>
public sealed class AdRevenueEventArgs(string? adUnitId, decimal amount, string currencyCode, AdRevenuePrecision precision) : EventArgs
{
    /// <summary>The ad unit that earned the revenue, when available.</summary>
    public string? AdUnitId { get; } = adUnitId;

    /// <summary>The amount in whole currency units. For example, 0.005 USD is half a cent.</summary>
    public decimal Amount { get; } = amount;

    /// <summary>The ISO 4217 currency code reported by the SDK.</summary>
    public string CurrencyCode { get; } = currencyCode;

    /// <summary>How the SDK determined the amount.</summary>
    public AdRevenuePrecision Precision { get; } = precision;
}

/// <summary>
/// The precision of an impression-level revenue amount.
/// </summary>
public enum AdRevenuePrecision
{
    /// <summary>The precision is unknown.</summary>
    Unknown,

    /// <summary>The amount is estimated from aggregated data.</summary>
    Estimated,

    /// <summary>The amount is based on a value supplied by the publisher.</summary>
    PublisherProvided,

    /// <summary>The amount is the precise value paid for the impression.</summary>
    Precise,
}
