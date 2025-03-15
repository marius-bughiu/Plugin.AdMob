namespace Plugin.AdMob;

/// <summary>
/// Manages a native ad instance.
/// </summary>
public interface INativeAd
{
    /// <summary>
    /// The ad unit ID.
    /// </summary>
    string AdUnitId { get; }

    /// <summary>
    /// Determines whether the ad is loaded or not.
    /// </summary>
    bool IsLoaded { get; }

    string Image => throw new NotImplementedException();

    /// <summary>
    /// Raised when an ad is loaded.
    /// </summary>
    event EventHandler OnAdLoaded;

    /// <summary>
    /// Raised when an ad request failed.
    /// </summary>
    event EventHandler<IAdError> OnAdFailedToLoad;

    /// <summary>
    /// Loads a native ad using the specified <see cref="AdUnitId" />.
    /// </summary>
    void Load() => throw new NotImplementedException();
}

internal partial class NativeAd : INativeAd
{
    public string AdUnitId { get; }

    public bool IsLoaded { get; private set; }

    public event EventHandler? OnAdLoaded;
    public event EventHandler<IAdError>? OnAdFailedToLoad;

    public NativeAd(string adUnitId)
    {
        AdUnitId = adUnitId;
    }
}
