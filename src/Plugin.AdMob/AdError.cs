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
}

internal class AdError : IAdError
{
    internal const string UnsupportedPlatformMessage = "Ads are only supported on Android and iOS.";

    internal const string AdExpiredMessage = "The ad expired before it was shown. Load a new one.";

    public AdError(string message)
    {
        Message = message;
        Debug.WriteLine($"[Plugin.AdMob] Ad error: {message}");
    }

    public string Message { get; }
}
