using System.Diagnostics;

namespace Plugin.AdMob;

/// <summary>
/// An error which occurred while loading or showing an ad.
/// </summary>
public interface IAdError
{
    /// <summary>
    /// The error message.
    /// </summary>
    string Message { get; }

    /// <summary>The native SDK error code, when available.</summary>
    long? Code => null;

    /// <summary>The domain of the native error, when the SDK supplies one.</summary>
    string? Domain => null;

    /// <summary>The ID of the associated ad response, when available.</summary>
    string? ResponseId => null;
}

internal class AdError : IAdError
{
    internal const string UnsupportedPlatformMessage = "Ads are only supported on Android and iOS.";

    internal const string AdExpiredMessage = "The ad expired before it was shown. Load a new one.";

    public AdError(string message)
        : this(message, null, null, null)
    {
    }

    public AdError(string message, long? code, string? domain, string? responseId)
    {
        Message = message;
        Code = code;
        Domain = domain;
        ResponseId = responseId;
        Debug.WriteLine($"[Plugin.AdMob] Ad error: {message}");
    }

    public AdError(Exception exception)
        : this($"{exception.GetType().FullName}: {exception.Message}")
    {
    }

    public string Message { get; }
    public long? Code { get; }
    public string? Domain { get; }
    public string? ResponseId { get; }
}
