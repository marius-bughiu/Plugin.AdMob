namespace Plugin.AdMob.Platforms.Android;

internal class AdListener : global::Android.Gms.Ads.AdListener
{
    public event EventHandler? AdLoaded;
    public event EventHandler<global::Android.Gms.Ads.AdError>? AdFailedToLoad;
    public event EventHandler? AdImpression;
    public event EventHandler? AdClicked;
    public event EventHandler? AdOpened;
    public event EventHandler? AdClosed;
    public event EventHandler? AdSwiped;

    public override void OnAdLoaded()
    {
        base.OnAdLoaded();
        AdLoaded?.Invoke(this, EventArgs.Empty);
    }

    public override void OnAdFailedToLoad(global::Android.Gms.Ads.LoadAdError error)
    {
        base.OnAdFailedToLoad(error);
        AdFailedToLoad?.Invoke(this, error);
    }

    public override void OnAdImpression()
    {
        base.OnAdImpression();
        AdImpression?.Invoke(this, EventArgs.Empty);
    }

    public override void OnAdClicked()
    {
        base.OnAdClicked();
        AdClicked?.Invoke(this, EventArgs.Empty);
    }

    public override void OnAdOpened()
    {
        base.OnAdOpened();
        AdOpened?.Invoke(this, EventArgs.Empty);
    }

    public override void OnAdClosed()
    {
        base.OnAdClosed();
        AdClosed?.Invoke(this, EventArgs.Empty);
    }

    public override void OnAdSwipeGestureClicked()
    {
        base.OnAdSwipeGestureClicked();
        AdSwiped?.Invoke(this, EventArgs.Empty);
    }
}
