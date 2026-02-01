namespace Plugin.AdMob;

/// <summary>
/// Displays a banner ad.
/// </summary>
public class BannerAd : ContentView
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

    /// <summary>
    /// Determines whether the ad is loaded or not.
    /// </summary>
    public static readonly BindableProperty IsLoadedProperty =
        BindableProperty.Create(nameof(IsLoaded), typeof(bool), typeof(BannerAd), false);

    /// <summary>
    /// Determines whether the ad is loaded or not.
    /// </summary>
    public bool IsLoaded
    {
        get { return (bool)GetValue(IsLoadedProperty); }
        private set { SetValue(IsLoadedProperty, value); }
    }

    /// <summary>
    /// The ad unit id.
    /// </summary>
    public static readonly BindableProperty AdUnitIdProperty =
        BindableProperty.Create(nameof(AdUnitId), typeof(string), typeof(BannerAd), null);

    /// <summary>
    /// The ad unit id.
    /// </summary>
    public string AdUnitId
    {
        get { return (string)GetValue(AdUnitIdProperty); }
        set { SetValue(AdUnitIdProperty, value); }
    }

    /// <summary>
    /// The desired ad size.
    /// </summary>
    public static readonly BindableProperty AdSizeProperty =
        BindableProperty.Create(nameof(AdSize), typeof(AdSize), typeof(BannerAd), null);

    /// <summary>
    /// The desired ad size.
    /// </summary>
    public AdSize? AdSize
    {
        get { return (AdSize?)GetValue(AdSizeProperty); }
        set { SetValue(AdSizeProperty, value); }
    }

    /// <summary>
    /// The desired ad width in density-independent pixels. Used when <see cref="AdSize" /> is set to <see cref="AdMob.AdSize.Custom" />.
    /// </summary>
    public static readonly BindableProperty AdWidthProperty =
        BindableProperty.Create("AdWidth", typeof(int), typeof(BannerAd), null);

    /// <summary>
    /// The desired ad width in density-independent pixels. Used when <see cref="AdSize" /> is set to <see cref="AdMob.AdSize.Custom" />.
    /// </summary>
    public int CustomAdWidth
    {
        get { return (int)GetValue(AdWidthProperty); }
        set { SetValue(AdWidthProperty, value); }
    }

    /// <summary>
    /// The desired ad height in density-independent pixels. Used when <see cref="AdSize" /> is set to <see cref="AdMob.AdSize.Custom" />.
    /// </summary>
    public static readonly BindableProperty AdHeightProperty =
        BindableProperty.Create("AdHeight", typeof(int), typeof(BannerAd), null);

    /// <summary>
    /// The desired ad height in density-independent pixels. Used when <see cref="AdSize" /> is set to <see cref="AdMob.AdSize.Custom" />.
    /// </summary>
    public int CustomAdHeight
    {
        get { return (int)GetValue(AdHeightProperty); }
        set { SetValue(AdHeightProperty, value); }
    }

    internal void RaiseOnAdLoaded(object? sender, EventArgs e)
    {
        IsLoaded = true;
        OnAdLoaded?.Invoke(sender, e);
    }

    internal void RaiseOnAdFailedToLoad(object? sender, IAdError e)
    {
        IsLoaded = false;
        OnAdFailedToLoad?.Invoke(sender, e);
    }
    internal void RaiseOnAdImpression(object? sender, EventArgs e) => OnAdImpression?.Invoke(sender, e);
    internal void RaiseOnAdClicked(object? sender, EventArgs e) => OnAdClicked?.Invoke(sender, e);
    internal void RaiseOnAdSwiped(object? sender, EventArgs e) => OnAdSwiped?.Invoke(sender, e);
    internal void RaiseOnAdOpened(object? sender, EventArgs e) => OnAdOpened?.Invoke(sender, e);
    internal void RaiseOnAdClosed(object? sender, EventArgs e) => OnAdClosed?.Invoke(sender, e);
}
