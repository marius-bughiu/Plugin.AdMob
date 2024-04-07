namespace Plugin.AdMob;

public class BannerAd : ContentView
{
    public event EventHandler OnAdLoaded;
    public event EventHandler<IAdError> OnAdFailedToLoad;
    public event EventHandler OnAdImpression;
    public event EventHandler OnAdClicked;
    public event EventHandler OnAdSwiped;
    public event EventHandler OnAdOpened;
    public event EventHandler OnAdClosed;

    public static readonly BindableProperty AdUnitIdProperty =
        BindableProperty.Create("AdUnitId", typeof(string), typeof(BannerAd), null);

    public string AdUnitId
    {
        get { return (string)GetValue(AdUnitIdProperty); }
        set { SetValue(AdUnitIdProperty, value); }
    }

    public static readonly BindableProperty AdSizeProperty =
        BindableProperty.Create("AdSize", typeof(AdSize), typeof(BannerAd), null);

    public AdSize? AdSize
    {
        get { return (AdSize?)GetValue(AdSizeProperty); }
        set { SetValue(AdSizeProperty, value); }
    }

    public static readonly BindableProperty AdWidthProperty =
        BindableProperty.Create("AdWidth", typeof(int), typeof(BannerAd), null);

    public int CustomAdWidth
    {
        get { return (int)GetValue(AdWidthProperty); }
        set { SetValue(AdWidthProperty, value); }
    }

    public static readonly BindableProperty AdHeightProperty =
        BindableProperty.Create("AdHeight", typeof(int), typeof(BannerAd), null);

    public int CustomAdHeight
    {
        get { return (int)GetValue(AdHeightProperty); }
        set { SetValue(AdHeightProperty, value); }
    }

    internal void RaiseOnAdLoaded(object sender, EventArgs e) => OnAdLoaded?.Invoke(sender, e);
    internal void RaiseOnAdFailedToLoad(object sender, IAdError e) => OnAdFailedToLoad?.Invoke(sender, e);
    internal void RaiseOnAdImpression(object sender, EventArgs e) => OnAdImpression?.Invoke(sender, e);
    internal void RaiseOnAdClicked(object sender, EventArgs e) => OnAdClicked?.Invoke(sender, e);
    internal void RaiseOnAdSwiped(object sender, EventArgs e) => OnAdSwiped?.Invoke(sender, e);
    internal void RaiseOnAdOpened(object sender, EventArgs e) => OnAdOpened?.Invoke(sender, e);
    internal void RaiseOnAdClosed(object sender, EventArgs e) => OnAdClosed?.Invoke(sender, e);
}
