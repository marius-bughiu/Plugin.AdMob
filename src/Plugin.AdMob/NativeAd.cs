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

    /// <summary>
    /// Returns text that identifies the advertiser. Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? Advertiser => throw new NotImplementedException();

    /// <summary>
    /// Returns body text. Apps are required to display this asset.
    /// </summary>
    string? Body => throw new NotImplementedException();

    /// <summary>
    /// Returns the ad's call to action (such as "Buy" or "Install"). Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? CallToAction => throw new NotImplementedException();

    /// <summary>
    /// Returns the primary text headline. Apps are required to display this asset.
    /// </summary>
    string? Headline => throw new NotImplementedException();

    //string? Icon => throw new NotImplementedException();

    /// <summary>
    /// Returns a the uri of a small image identifying the advertiser. Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? IconUri => throw new NotImplementedException();

    //string? Images => throw new NotImplementedException();

    /// <summary>
    /// Returns a the uri of a large image identifying the advertiser. Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? ImageUri => throw new NotImplementedException();

    /// <summary>
    /// For ads about apps, returns a string representing how much the app costs. Apps are not required to display this asset, though it's recommended.
    /// </summary>
    string? Price => throw new NotImplementedException();

    /// <summary>
    /// For ads about apps, returns a star rating from 0 to 5 representing how many stars the app has in the store offering it. Apps are not required to display this asset, though it's recommended.
    /// </summary>
    double? StarRating => throw new NotImplementedException();

    /// <summary>
    /// For ads about apps, returns the name of the store offering the app for download. For example, "Google Play". Apps are not required to display this asset, though it's recommended.
    /// </summary>
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

internal partial class NativeAd(string adUnitId) : INativeAd
{
    public string AdUnitId { get; } = adUnitId;

    public bool IsLoaded { get; private set; }

    public event EventHandler? OnAdLoaded;
    public event EventHandler<IAdError>? OnAdFailedToLoad;
    public event EventHandler? OnAdImpression;
    public event EventHandler? OnAdClicked;
    public event EventHandler? OnAdSwiped;
    public event EventHandler? OnAdOpened;
    public event EventHandler? OnAdClosed;
}
