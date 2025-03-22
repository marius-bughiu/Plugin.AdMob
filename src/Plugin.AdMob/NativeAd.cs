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

    string? Advertiser => throw new NotImplementedException();

    string? Body => throw new NotImplementedException();

    string? CallToAction => throw new NotImplementedException();

    string? Headline => throw new NotImplementedException();

    //string? Icon => throw new NotImplementedException();

    string? IconUri => throw new NotImplementedException();

    //string? Images => throw new NotImplementedException();

    string? ImageUri => throw new NotImplementedException();

    string? Price => throw new NotImplementedException();

    double? StarRating => throw new NotImplementedException();

    string? Store => throw new NotImplementedException();

    /// <summary>
    /// Raised when an ad is loaded.
    /// </summary>
    event EventHandler OnAdLoaded;

    /// <summary>
    /// Raised when an ad request failed.
    /// </summary>
    event EventHandler<IAdError> OnAdFailedToLoad;

    /// <summary>
    /// Raised when an impression is recorded for an ad.
    /// </summary>
    event EventHandler? OnAdImpression;

    /// <summary>
    /// Raised when a click is recorded for an ad.
    /// </summary>
    event EventHandler? OnAdClicked;

    /// <summary>
    /// Raised when a swipe gesture on an ad is recorded as a click. Supported only by Android.
    /// </summary>
    event EventHandler? OnAdSwiped;

    /// <summary>
    /// Raised when an ad opens an overlay that covers the screen.
    /// </summary>
    event EventHandler? OnAdOpened;

    /// <summary>
    /// Raised when the user is about to return to the application after clicking on an ad.
    /// </summary>
    event EventHandler? OnAdClosed;

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
    public event EventHandler? OnAdImpression;
    public event EventHandler? OnAdClicked;
    public event EventHandler? OnAdSwiped;
    public event EventHandler? OnAdOpened;
    public event EventHandler? OnAdClosed;

    public NativeAd(string adUnitId)
    {
        AdUnitId = adUnitId;
    }
}
