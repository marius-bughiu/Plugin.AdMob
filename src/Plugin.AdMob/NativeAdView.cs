namespace Plugin.AdMob;

/// <summary>
/// Container view used for displaying a native ad.
/// </summary>
public class NativeAdView : ContentView
{
    /// <summary>
    /// Raised when an ad is received.
    /// </summary>
    public event EventHandler? OnAdLoaded;

    /// <summary>
    /// Raised when an ad request failed.
    /// </summary>
    public event EventHandler<IAdError>? OnAdFailedToLoad;

    /// <summary>
    /// Raised when an impression is recorded for an ad.
    /// </summary>
    public event EventHandler? OnAdImpression;

    /// <summary>
    /// Raised when a click is recorded for an ad.
    /// </summary>
    public event EventHandler? OnAdClicked;

    /// <summary>
    /// Raised when a swipe gesture on an ad is recorded as a click. Supported only by Android.
    /// </summary>
    public event EventHandler? OnAdSwiped;

    /// <summary>
    /// Raised when an ad opens an overlay that covers the screen.
    /// </summary>
    public event EventHandler? OnAdOpened;

    /// <summary>
    /// Raised when the user is about to return to the application after clicking on an ad.
    /// </summary>
    public event EventHandler? OnAdClosed;

    public INativeAd Ad { get; private set; }

    public IElement AdContent { get; private set; }

    public NativeAdView(INativeAd ad, ContentView adContent)
    {
        Ad = ad;
        AdContent = adContent;
    }

    internal void RaiseOnAdLoaded(object? sender, EventArgs e) => OnAdLoaded?.Invoke(sender, e);
    internal void RaiseOnAdFailedToLoad(object? sender, IAdError e) => OnAdFailedToLoad?.Invoke(sender, e);
    internal void RaiseOnAdImpression(object? sender, EventArgs e) => OnAdImpression?.Invoke(sender, e);
    internal void RaiseOnAdClicked(object? sender, EventArgs e) => OnAdClicked?.Invoke(sender, e);
    internal void RaiseOnAdSwiped(object? sender, EventArgs e) => OnAdSwiped?.Invoke(sender, e);
    internal void RaiseOnAdOpened(object? sender, EventArgs e) => OnAdOpened?.Invoke(sender, e);
    internal void RaiseOnAdClosed(object? sender, EventArgs e) => OnAdClosed?.Invoke(sender, e);
}
