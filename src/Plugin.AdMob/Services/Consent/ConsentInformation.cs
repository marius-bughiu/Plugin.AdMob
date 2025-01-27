namespace Plugin.AdMob;

/// <summary>
/// Consent status values.
/// </summary>
public enum ConsentStatus
{
    /// <summary>
    /// Consent status is unknown.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// User consent not required.
    /// </summary>
    NotRequired = 1,

    /// <summary>
    /// User consent required but not yet obtained.
    /// </summary>
    Required = 2,

    /// <summary>
    /// User consent obtained.
    /// </summary>
    Obtained = 3
}

/// <summary>
/// A snapshot in time of the user's consent information.
/// </summary>
public interface IConsentInformation
{
    /// <summary>
    /// The user's consent status.
    /// </summary>
    public ConsentStatus Status { get; }

    /// <summary>
    /// Determines whether all the criteria for requesting ads is met.
    /// </summary>
    public bool CanRequestAds { get; }
}

internal class ConsentInformation(ConsentStatus status, bool canRequestAds) 
    : IConsentInformation
{
    public ConsentStatus Status { get; } = status;

    public bool CanRequestAds { get; } = canRequestAds;
}
